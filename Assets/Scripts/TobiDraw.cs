using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

/*
 * This class can draw lines/circles using LineRenderer Components in Game/Scene-View
 * It uses an object pool to help performance.
 *
 * How to use:
 * 1. Create an Object and add TobiDraw.cs as component
 * 2. Set lineMaterial
 * 3. draw stuff using TobiDraw.tobiDraw.DrawX();
 */
public class TobiDraw : MonoBehaviour
{
    private Stack<LineRenderer> inactiveLineRenderer;
    private Stack<LineRenderer> activeLineRenderer;
    public static TobiDraw tobiDraw;
    public Material lineMaterial;
    private void Start()
    {
        inactiveLineRenderer = new Stack<LineRenderer>();
        activeLineRenderer = new Stack<LineRenderer>();
        tobiDraw = this;
    }

    public void DrawLine(Vector3 a, Vector3 b, Color c, float width = 1f)
    {
        var l = GetNewLineObject();
        l.positionCount = 2;
        l.SetPosition(0, a);
        l.SetPosition(1, b);
        l.startColor = l.endColor = c;
        l.startWidth = l.endWidth = width;
    }
    
    private void Update()
    {
        // reset old lines
        var n = activeLineRenderer.Count;
        for (var i = 0; i < n; i++)
        {
            var l = activeLineRenderer.Pop();
            inactiveLineRenderer.Push(l);
            l.gameObject.SetActive(false);
        }
    }

    public LineRenderer GetNewLineObject()
    {
        if (inactiveLineRenderer.Count > 0)
        {
            // reuse existing LineRenderer-Object
            var l = inactiveLineRenderer.Pop();
            activeLineRenderer.Push(l);
            l.gameObject.SetActive(true);
            return l;
        }
        else
        {
            // create new LineRenderer-Object
            var obj = new GameObject("LineRendererObject");
            obj.transform.parent = gameObject.transform;
            var l = obj.AddComponent<LineRenderer>();
            activeLineRenderer.Push(l);
            return l;
        }
    }
}