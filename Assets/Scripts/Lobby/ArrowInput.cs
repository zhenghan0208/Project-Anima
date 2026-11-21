
using UnityEngine;

public class ArrowInput : MonoBehaviour
{
    [Header("Hit Line Settings")]
    public RectTransform[] hitLines;

    [Header("Hit Detection")]
    public float hitRange = 50f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            CheckHit(0);

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            CheckHit(1);

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            CheckHit(2);

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            CheckHit(3);
    }

    void CheckHit(int laneIndex)
    {
        if (hitLines == null || laneIndex >= hitLines.Length)
            return;

        RectTransform hitLine = hitLines[laneIndex];

        if (hitLine == null || hitLine.parent == null)
            return;

        // Search arrows inside the Lane, not inside HitLine
        FallingArrow[] arrows =
            hitLine.parent.GetComponentsInChildren<FallingArrow>();

        FallingArrow closestArrow = null;
        float closestDistance = float.MaxValue;

        foreach (FallingArrow arrow in arrows)
        {
            if (arrow == null || arrow.IsHit)
                continue;

            float distance = Mathf.Abs(
                arrow.CurrentY - hitLine.anchoredPosition.y
            );

            if (distance <= hitRange && distance < closestDistance)
            {
                closestDistance = distance;
                closestArrow = arrow;
            }
        }

        if (closestArrow != null)
        {
            closestArrow.Hit();
            Debug.Log("Hit!");
        }
        else
        {
            Debug.Log("Miss!");
        }
    }
}