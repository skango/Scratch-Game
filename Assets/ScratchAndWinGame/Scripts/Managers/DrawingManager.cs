using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawingManager : MonoBehaviour
{
    public static DrawingManager instance;

    List<Vector3> positions = new List<Vector3>();
    List<Vector3> ResampledPositions = new List<Vector3>();
    public GameObject LineRenderPrefab;
    private LineRenderer CurrentLineRender;
    public List<LineRenderer> myLineInstances = new List<LineRenderer>();

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    [ContextMenu("Clear Instances")]
    public void ClearInstances()
    {
        foreach (var item in myLineInstances)
        {
            Destroy(item.gameObject);
        }

        positions.Clear();
        ResampledPositions.Clear();
        myLineInstances.Clear();
    }

    // Create New Lne Renderer
    public void CreateLineWithSamePont(PointerEventData eventData)
    {
        CurrentLineRender = Instantiate(LineRenderPrefab).GetComponent<LineRenderer>();
        CurrentLineRender.SetPosition(0, new Vector3(eventData.pointerCurrentRaycast.worldPosition.x, eventData.pointerCurrentRaycast.worldPosition.y, 0));
        CurrentLineRender.SetPosition(1, new Vector3(eventData.pointerCurrentRaycast.worldPosition.x, eventData.pointerCurrentRaycast.worldPosition.y, 0));
        myLineInstances.Add(CurrentLineRender);
        RaycastMe(eventData);
    }


    // Updating Line Position
    public void UpdateLinePosition(PointerEventData eventData)
    {
        CurrentLineRender.positionCount = CurrentLineRender.positionCount + 1;
        CurrentLineRender.SetPosition(CurrentLineRender.positionCount - 1, new Vector3(eventData.pointerCurrentRaycast.worldPosition.x, eventData.pointerCurrentRaycast.worldPosition.y, 0));
        CreateLineWithSamePont(eventData);
    }

    // Main Drawing method with LineRenderer
    public void Draw(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.isValid == true)
        {
            if (CurrentLineRender == null) CreateLineWithSamePont(eventData);
            else
            {
                Vector3 oldPosition = CurrentLineRender.GetPosition(0);
                Vector3 currentPosition = eventData.pointerCurrentRaycast.worldPosition;

                UpdateLinePosition(eventData);

                Vector3 Heading = currentPosition - oldPosition;
                float Distance = Heading.magnitude;
                Vector3 Direction = Heading.normalized;

                float MinDistance = 0.5f;

                // subsampling the Line
                if(Distance >= MinDistance)
                {
                    float howManyRaycasts = Distance / MinDistance;

                    for (int i = 0; i < howManyRaycasts; i++)
                    {
                        Vector3 RaycastPosition = oldPosition + (Direction * i * MinDistance);
                        eventData.position = eventData.enterEventCamera.WorldToScreenPoint(RaycastPosition);
                        RaycastMe(eventData, true);
                    }
                }
            }
        }
    }

    // Draw gizmos for debugging
    private void OnDrawGizmos()
    {
        foreach (var pos in positions)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(pos, 0.15f);
        }

        foreach (var pos in ResampledPositions)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(pos, 0.1f);
        }
    }

    // When we start dragging the curent Line renderer will be null
    public void OnBeginDrag(PointerEventData eventData)
    {
        CurrentLineRender = null;
    }


    // Raycasting which detects samples even if they are overlapping
    private void RaycastMe(PointerEventData eventData, bool resampled = false)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (resampled == true) ResampledPositions.Add(result.worldPosition);
            else positions.Add(result.worldPosition);

            if (result.gameObject.TryGetComponent(out Sample mySample))
            {
                mySample.VisitSample();
            }
        }
    }
}
