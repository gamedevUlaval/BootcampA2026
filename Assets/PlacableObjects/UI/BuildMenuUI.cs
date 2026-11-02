using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildMenuUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("This menu's own MultiplayerEventSystem (on the same object).")]
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject panel;
    [SerializeField] private RectTransform listContainer;
    [SerializeField] private Button rowPrefab;

    [Header("Look")]
    [SerializeField] private Color buildableColor = new Color(0.4f, 1f, 0.4f);
    [SerializeField] private Color notBuildableColor = new Color(1f, 0.4f, 0.4f);

    [Header("World placement")]
    [Tooltip("Menu height as a fraction of the screen height.")]
    [SerializeField, Range(0.1f, 1f)] private float screenHeightFraction = 0.5f;
    [Tooltip("Offset from the player as a fraction of screen height. X = right (mirrored for odd player indexes), Y = up.")]
    [SerializeField] private Vector2 screenOffset = new Vector2(0.08f, 0f);
    [SerializeField] private float towardCamera = 1f;

    private BuilderPlayerComponent builder;
    private Transform followTarget;
    private Camera cam;
    private bool onLeftSide;

    private readonly List<Button> rows = new List<Button>();
    private readonly List<TMP_Text> labels = new List<TMP_Text>();

    public bool IsOpen => panel != null && panel.activeSelf;

    private void Awake()
    {
        builder = GetComponentInParent<BuilderPlayerComponent>();
        if (builder == null)
        {
            Debug.LogError("BuildMenuUI must be a child of an object with BuilderPlayerComponent", this);
            enabled = false;
            return;
        }

        followTarget = builder.transform;
        if (eventSystem == null) eventSystem = GetComponent<EventSystem>();
    }

    private void OnEnable()
    {
        if (builder != null) builder.OnBuilt += HandleBuilt;
    }

    private void OnDisable()
    {
        if (builder != null) builder.OnBuilt -= HandleBuilt;
    }

    private void Start()
    {
        transform.SetParent(null, true);

        onLeftSide = builder.PlayerIndex == 0;
        ((RectTransform)transform).pivot = new Vector2(onLeftSide ? 1f : 0f, 0.5f);

        panel.SetActive(false);
    }

    public void Toggle()
    {
        if (IsOpen) Close(); else Open();
    }

    public void Open()
    {
        BuildRows();
        panel.SetActive(true);

        if (rows.Count > 0 && eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(rows[0].gameObject);
        }
    }

    public void Close()
    {
        panel.SetActive(false);
        if (eventSystem != null) eventSystem.SetSelectedGameObject(null);
    }

    private void LateUpdate()
    {
        if (!IsOpen || followTarget == null) return;

        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        Transform camT = cam.transform;

        float visibleHeight;
        if (cam.orthographic)
        {
            visibleHeight = cam.orthographicSize * 2f;
        }
        else
        {
            float distance = Vector3.Dot(followTarget.position - camT.position, camT.forward);
            visibleHeight = 2f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        }

        RectTransform rt = (RectTransform)transform;
        float scale = (visibleHeight * screenHeightFraction) / rt.rect.height;
        transform.localScale = new Vector3(scale, scale, scale);

        float side = onLeftSide ? -1f : 1f;
        transform.position = followTarget.position
                           + camT.right * (screenOffset.x * visibleHeight * side)
                           + camT.up * (screenOffset.y * visibleHeight)
                           - camT.forward * towardCamera;

        transform.rotation = camT.rotation;
    }

    private void BuildRows()
    {
        foreach (Button row in rows)
        {
            if (row != null) Destroy(row.gameObject);
        }
        rows.Clear();
        labels.Clear();

        foreach (BuildableBlueprintScriptableObject blueprint in builder.AvailableBlueprints)
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
        IReadOnlyList<BuildableBlueprintScriptableObject> blueprints = builder.AvailableBlueprints;

        for (int i = 0; i < rows.Count; i++)
        {
            bool buildable = builder.IsBuildable(blueprints[i]);
            labels[i].text = FormatLabel(blueprints[i]);
            labels[i].color = buildable ? buildableColor : notBuildableColor;
        }
    }

    private string FormatLabel(BuildableBlueprintScriptableObject blueprint)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<b>").Append(blueprint.name).Append("</b>");
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
        builder.TryBuild(blueprint);
    }

    private void HandleBuilt(BuildableBlueprintScriptableObject blueprint)
    {
        if (IsOpen) RefreshRows();
    }
}