using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject panel;
    [Tooltip("Parent of the rows. Add a VerticalLayoutGroup to it.")]
    [SerializeField] private RectTransform listContainer;
    [Tooltip("A Button with a TMP_Text child.")]
    [SerializeField] private Button rowPrefab;
    [SerializeField] private TMP_Text feedbackText;

    private BuilderPlayerComponent builder;

    [Header("Look")]
    [SerializeField] private Color buildableColor = new Color(0.4f, 1f, 0.4f);
    [SerializeField] private Color notBuildableColor = new Color(1f, 0.4f, 0.4f);

    [Header("World placement")]
    [Tooltip("X = screen-right of the player, Y = screen-up of the player (camera-relative).")]
    [SerializeField] private Vector2 screenOffset = new Vector2(3f, 1f);
    [Tooltip("Pulls the menu toward the camera so it never clips into the scene.")]
    [SerializeField] private float towardCamera = 1f;

    private Transform followTarget;
    private Camera cam;

    private readonly List<Button> rows = new List<Button>();
    private readonly List<TMP_Text> labels = new List<TMP_Text>();

    public bool IsOpen => panel != null && panel.activeSelf;

    private void Start()
    {
        panel.SetActive(false);
    }

    private void LateUpdate()
    {
        if (!IsOpen || followTarget == null) return;

        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        Transform camT = cam.transform;

        transform.position = followTarget.position
                           + camT.right * screenOffset.x
                           + camT.up * screenOffset.y
                           - camT.forward * towardCamera;

        transform.rotation = camT.rotation;
    }

    private void OnEnable()
    {
        Subscribe(builder);
    }

    private void OnDisable()
    {
        Unsubscribe(builder);
    }

    public void SetBuilder(BuilderPlayerComponent newBuilder)
    {
        if (newBuilder == builder) return;

        Unsubscribe(builder);
        builder = newBuilder;
        Subscribe(builder);

        followTarget = builder != null ? builder.transform : null;

        if (IsOpen) Close();
    }

    private void Subscribe(BuilderPlayerComponent target)
    {
        if (target != null) target.OnBuilt += HandleBuilt;
    }

    private void Unsubscribe(BuilderPlayerComponent target)
    {
        if (target != null) target.OnBuilt -= HandleBuilt;
    }

    public void Toggle()
    {
        if (builder == null) return;
        if (IsOpen) Close(); else Open();
    }

    public void Open()
    {
        if (builder == null) return;
        BuildRows();
        panel.SetActive(true);
        SetFeedback(string.Empty);

        if (rows.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(rows[0].gameObject);
        }
    }

    public void Close()
    {
        panel.SetActive(false);
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void BuildRows()
    {
        foreach (Button row in rows)
        {
            if (row != null) Destroy(row.gameObject);
        }
        rows.Clear();
        labels.Clear();

        foreach (BuildableBlueprintScriptableObject blueprint in builder.availableBlueprints)
        {
            BuildableBlueprintScriptableObject captured = blueprint;
            Button row = Instantiate(rowPrefab, listContainer);
            row.onClick.AddListener(() => OnRowClicked(captured));

            rows.Add(row);
            labels.Add(row.GetComponentInChildren<TMP_Text>());
        }

        RefreshRows();
    }

    private void RefreshRows()
    {
        IReadOnlyList<BuildableBlueprintScriptableObject> blueprints = builder.availableBlueprints;

        for (int i = 0; i < rows.Count; i++)
        {
            BuildableBlueprintScriptableObject blueprint = blueprints[i];
            bool buildable = builder.IsBuildable(blueprint);

            labels[i].text = FormatLabel(blueprint, buildable);
            labels[i].color = buildable ? buildableColor : notBuildableColor;
        }
    }

    private string FormatLabel(BuildableBlueprintScriptableObject blueprint, bool buildable)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(blueprint.name);
        sb.Append(buildable ? "  [Available]" : "  [Missing resources]");
        sb.Append("\n<size=70%>");

        bool first = true;
        foreach (ResourceEntry cost in blueprint.recipe)
        {
            if (!first) sb.Append(", ");
            sb.Append(cost.resourceType).Append(" x").Append(cost.value);
            first = false;
        }

        sb.Append("</size>");
        return sb.ToString();
    }

    private void OnRowClicked(BuildableBlueprintScriptableObject blueprint)
    {
        if (builder.TryBuild(blueprint))
        {
            SetFeedback($"Built {blueprint.name}");
        }
        else
        {
            SetFeedback($"Not enough resources for {blueprint.name}");
        }
    }

    private void HandleBuilt(BuildableBlueprintScriptableObject blueprint)
    {
        if (IsOpen) RefreshRows();
    }

    private void SetFeedback(string message)
    {
        if (feedbackText != null) feedbackText.text = message;
    }
}