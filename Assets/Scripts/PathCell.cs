using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCell
{
    // A* algorithm
    public int X { get; }
    public int Z { get; }
    float f, g, h;
    PathCell parent;

    public PathCell(int x, int z) {
        X = x;
        Z = z;
        f = g = h = 0f;
        parent = null;
    }

    // Functions for A* algorithm
    public float GetF() {
        return f;
    }

    public void SetF(float f) {
        this.f = f;
    }

    public float GetG() {
        return g;
    }

    public void SetG(float g) {
        this.g = g;
    }

    public float GetH() {
        return h;
    }

    public void SetH(float h) {
        this.h = h;
    }

    public PathCell GetParent() {
        return parent;
    }

    public void SetParent(PathCell c) {
        this.parent = c;
    }
}
