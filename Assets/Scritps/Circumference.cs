﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GizmoType {
	sphere,
	line
}

public struct GizmoToDraw {
	public GizmoType gizmoType;
	public Vector3 from;
	public Vector3 to;
	public float size;
	public Color color;
	
	public GizmoToDraw (GizmoType type, Vector3 origen, float radius, Color col) {
		gizmoType = type;
		from = origen;
		size = radius;
		color = col;
		to = Vector3.zero;
	}

	public GizmoToDraw (GizmoType type, Vector3 origen, Vector3 destiny, Color col) {
		gizmoType = type;
		from = origen;
		to = destiny;
		size = 0;
		color = col;
	}
}

[RequireComponent (typeof (CircleCollider2D))]
public class Circumference : MonoBehaviour {
	public CircleCollider2D cc;
	private float radius;
	public Vector3 GetPosition() { return transform.position; }
	public float GetRadius() { return radius * transform.lossyScale.x; }

	public void Awake () {
		cc = GetComponent<CircleCollider2D>();
		radius = cc.radius;
		Initialize();
	}

	public virtual void Initialize(){}
}
