using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// Makes Sure that the object the script is attached to has a Mesh Filter
[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshRenderer))]


public class Cube : MonoBehaviour
{
	MeshFilter meshFilter;
	// adds a Mesh filter called meshFilter so we can access it from the script
	Mesh mesh;
	// Mesh called mesh so that we can access it from the script

	public List<Vector4> originalVerts; // Creates the list that will hold the original vector 3/4 co-ordinates of the shape
	[HideInInspector] public List<Vector4> rotatedVerts; // Creates a list that will hold the co-ordinates of our rotated shape, this list won't be shown in the inspector

	public List<Vector3> originalTris; // Creates a list for the original Tris

	// creates a list of our tris, verts and UVs
	public List<Vector3> verts;
	public List<int> tris;
	public List<Vector2> uvs;

	public List<Axis4D> rotationOrder; // Adds a list to hold the order of our rotations using Axis 4d
	public Dictionary<Axis4D, float> rotation; // Creates a Dictionary to hold our Axis 4d and a float value rotation

	void Start ()
	{
		rotationOrder = new List<Axis4D> (); // Makes the rotationOrder list = to this list
		rotationOrder.Add (Axis4D.yz); // Adds the yz axis to our list
		rotationOrder.Add (Axis4D.xw); // Adds the xw axis to our list
		rotationOrder.Add (Axis4D.yw); // Adds the yw axis to our list
		rotationOrder.Add (Axis4D.zw); // Adds the zw axis to our list
		rotationOrder.Add (Axis4D.xy); // Adds the xy axis to our list
		rotationOrder.Add (Axis4D.xz); // Adds the xz axis to our list

		rotation = new Dictionary<Axis4D, float> (); // Makes our rotation Dictionary = to this dictionary
		rotation.Add (Axis4D.xy, 0f); // adds the axis xy with a float of 0f
		rotation.Add (Axis4D.xz, 0f); // adds the axis xz with a float of 0f
		rotation.Add (Axis4D.xw, 0f); // adds the axis xw with a float of 0f
		rotation.Add (Axis4D.yz, 0f); // adds the axis yz with a float of 0f
		rotation.Add (Axis4D.yw, 0f); // adds the axis yw with a float of 0f
		rotation.Add (Axis4D.zw, 0f); // adds the axis zw with a float of 0f


		meshFilter = GetComponent<MeshFilter> (); // Makes the meshFilter we defined earlier = the MeshFilter on the GameObject
		mesh = meshFilter.sharedMesh; // Makes the mesh we defined earlier = meshFilter.sharedMesh
		if (mesh == null) { // Checks if theres a mesh, if not it creates a new one
			meshFilter.mesh = new Mesh ();
			mesh = meshFilter.sharedMesh;
		}

		/* this adds our co-ordinates of our shape, we enter the co-ordinates here for each vertex of the shape. 
		   When we put them in, each item in the list is automatically assigned a number, starting at 0 and increasing by one.
		   When we go to create our shape, in the Draw function, then we will use these to tell the script the positions of the shapes faces.*/
		originalVerts = new List<Vector4> () {
			new Vector3 (0, 1, 0),
			new Vector3 (0, 1, 1),
			new Vector3 (1, 1, 1),
			new Vector3 (1, 1, 0),
			new Vector3 (0, 0, 0),
			new Vector3 (0, 0, 1),
			new Vector3 (1, 0, 1),
			new Vector3 (1, 0, 0)
		};

		ResetVertices (); // Runs the ResetVertices function
	}

	Vector3 cameraPos = new Vector3 (0, 0, -6); // Makes a new vector three for the position of the camera
	void Update () // This function runs every frame
	{
		cameraPos.z += Input.GetAxis ("Mouse ScrollWheel") * 2f; // Sets the z position of the camera based on the scroll whele
		Camera.main.transform.position = cameraPos; // Sets the camera position to the cameraPos vector
		Camera.main.orthographicSize = -(cameraPos.z + 4); // Sets the orthographic camera size
		/*
        To be honest I can't really remember why this is still here, but we'll leave it just in case
        
        /Rotate(Axis4D.yw,0.06f);
        //Rotate(Axis4D.yz,0);
        
	  */
		DrawCube (); // Runs the DrawCube function
	}

	bool freezeRotation = false; // Creates a boolean called freezeRotation and sets it to false

	void OnGUI () // The Function that handles our GUI
	{

		/* This pretty much just makes our GUI... its pretty self explanatory...
		   It mainly makes sliders, but there are a few toggles there as well... */
		freezeRotation = GUI.Toggle (new Rect (25, 15, 200, 30), freezeRotation, "Freeze Rotation");

		GUI.Label (new Rect (25, 25, 100, 30), "XY");
		rotation [Axis4D.xy] = GUI.HorizontalSlider (new Rect (25, 50, 100, 30), Mathf.Repeat (rotation [Axis4D.xy], 360f), 0.0F, 360.0F);
		GUI.Label (new Rect (25, 75, 100, 30), "XZ");
		rotation [Axis4D.xz] = GUI.HorizontalSlider (new Rect (25, 100, 100, 30), Mathf.Repeat (rotation [Axis4D.xz], 360f), 0.0F, 360.0F);
		GUI.Label (new Rect (25, 125, 100, 30), "XW");
		rotation [Axis4D.xw] = GUI.HorizontalSlider (new Rect (25, 150, 100, 30), Mathf.Repeat (rotation [Axis4D.xw], 360f), 0.0F, 360.0F);
		GUI.Label (new Rect (25, 175, 100, 30), "YZ");
		rotation [Axis4D.yz] = GUI.HorizontalSlider (new Rect (25, 200, 100, 30), Mathf.Repeat (rotation [Axis4D.yz], 360f), 0.0F, 360.0F);
		GUI.Label (new Rect (25, 225, 100, 30), "YW");
		rotation [Axis4D.yw] = GUI.HorizontalSlider (new Rect (25, 250, 100, 30), Mathf.Repeat (rotation [Axis4D.yw], 360f), 0.0F, 360.0F);
		GUI.Label (new Rect (25, 275, 100, 30), "ZW");
		rotation [Axis4D.zw] = GUI.HorizontalSlider (new Rect (25, 300, 100, 30), Mathf.Repeat (rotation [Axis4D.zw], 360f), 0.0F, 360.0F);

		GUI.Label (new Rect (25, 325, 200, 30), "Zoom with the Mouse Wheel");
		Camera.main.orthographic = GUI.Toggle (new Rect (25, 375, 200, 30), Camera.main.orthographic, "Orth Camera");

		if (!freezeRotation) { // If the freezeRotation variable is true, it sets values in the rotate dictionary
			Rotate (Axis4D.xy, 0.1f);
			Rotate (Axis4D.xz, 0.15f);
			Rotate (Axis4D.xw, 0.6f);
			Rotate (Axis4D.yw, 0.3f);
			Rotate (Axis4D.yz, 0.45f);
			Rotate (Axis4D.zw, 0.5f);
		}

		ApplyRotationToVerts (); // Runs the ApplyRotationToVerts functions
	}

	void DrawCube () //Creates the Draw Cube Function
	{
		mesh.Clear (); // Clears the current mesh

		// Creates 3 lists, for verts, tris and uvs
		verts = new List<Vector3> ();
		tris = new List<int> ();
		uvs = new List<Vector2> ();

		/*	This part of the function creates the faces of our script. The Values are very specific. 
		 *  Notice there is four parameters, if you fill in all four with different values, It will create a square face.
		 *  If you want to have triangular faces, you can just put the same value in the 4th parameter as the 4th.
		 *  The numbers come from the OriginalVerts list we created earlier, and like I said, each number in the list is from 0, so if we wanted to make a face from the
		 *  first 4 vertices's, I would just input the number in the list  to each parameter.
		 *  It uses the CreatePlane function to make the faces.
		 */
		CreatePlane (rotatedVerts [0], rotatedVerts [1], rotatedVerts [2], rotatedVerts [3]);
		CreatePlane (rotatedVerts [0], rotatedVerts [1], rotatedVerts [4], rotatedVerts [5]);
		CreatePlane (rotatedVerts [0], rotatedVerts [3], rotatedVerts [4], rotatedVerts [7]);
		CreatePlane (rotatedVerts [3], rotatedVerts [2], rotatedVerts [6], rotatedVerts [7]);
		CreatePlane (rotatedVerts [2], rotatedVerts [1], rotatedVerts [5], rotatedVerts [6]);
		CreatePlane (rotatedVerts [4], rotatedVerts [5], rotatedVerts [6], rotatedVerts [7]);

	}
		

	void CreatePlane (Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4) // Our CreatePlane Function, It takes 4 Vectors
	{
		Vector2 uv0 = Vector2.zero; // Makes a vector 2 uv0 = a Vector 2 of (0,0)
		Vector2 uv1 = Vector2.zero; // Makes a vector 2 uv1 = a Vector 2 of (0,0)
		Vector2 uv2 = Vector2.zero; // Makes a vector 2 uv2 = a Vector 2 of (0,0)


		List<Vector3> newVerts = new List<Vector3> () { // Creates a list of vector3's called newVerts, to hold the new vertexes, and sets it to these values
			p1, p3, p2,
			p1, p2, p4,
			p2, p3, p4,
			p1, p4, p3
		};

		verts.AddRange (newVerts); // Adds the list of newVerts to the verts list
		mesh.vertices = verts.ToArray (); // Assigns the meshes vertices's to the array of the verts list

		int t = tris.Count; // Creates an integer t and makes its value = to the number of elements in the tris list

		// Creates a for loop that adds the integers j and t. the loop has the parameters that make an int j and make it = 0
		for (int j = 0; j < 12; j++) {
			tris.Add (j + t);
		}

		mesh.SetTriangles (tris.ToArray (), 0); // Sets the triangle list of the sub mesh = the elements of the tris list

		// Sets the value of the uvs we created earlier
		uv0 = new Vector2 (0, 0);
		uv1 = new Vector2 (1, 0);
		uv2 = new Vector2 (0.5f, 1);

		// Adds these values to the end of the uvs list
		uvs.AddRange (new List<Vector2> () {
				uv0, uv1, uv2,
				uv0, uv1, uv2,
				uv0, uv1, uv2,
				uv0, uv1, uv2
			}
		);
		mesh.uv = uvs.ToArray (); // Sets the texture co-ordinates (uvs) = the elements of the uvs list

		mesh.RecalculateNormals (); // Recalculates the Normals of the mesh from the triangles and vertices's
		mesh.RecalculateBounds (); // Recalculates the bounding volume of the mesh from the vertices's
		mesh.Optimize (); // Optimizes the mesh for display
	}

	Vector4 GetRotatedVertex (Axis4D axis, Vector4 v, float s, float c) // Creates a vector 4 that holds the axis from our list of axis's, a vector4, and two floats
	{
		switch (axis) {
		// Creates our different cases for each rotations
		case Axis4D.xy:
			return RotateAroundXY (v, s, c);
		case Axis4D.xz:
			return RotateAroundXZ (v, s, c);
		case Axis4D.xw:
			return RotateAroundXW (v, s, c);
		case Axis4D.yz:
			return RotateAroundYZ (v, s, c);
		case Axis4D.yw:
			return RotateAroundYW (v, s, c);
		case Axis4D.zw:
			return RotateAroundZW (v, s, c);
		}

		return new Vector4 (0, 0, 0, 0); // Returns a Vector 4
	}

	// These vectors do the calculations for each of our rotations

	Vector4 RotateAroundXY (Vector4 v, float s, float c)
	{
		float tmpX = c * v.x + s * v.y;
		float tmpY = -s * v.x + c * v.y;
		return new Vector4 (tmpX, tmpY, v.z, v.w);
	}

	Vector4 RotateAroundXZ (Vector4 v, float s, float c)
	{
		float tmpX = c * v.x + s * v.z;
		float tmpZ = -s * v.x + c * v.z;
		return new Vector4 (tmpX, v.y, tmpZ, v.w);
	}

	Vector4 RotateAroundXW (Vector4 v, float s, float c)
	{
		float tmpX = c * v.x + s * v.w;
		float tmpW = -s * v.x + c * v.w;
		return new Vector4 (tmpX, v.y, v.z, tmpW);
	}

	Vector4 RotateAroundYZ (Vector4 v, float s, float c)
	{
		float tmpY = c * v.y + s * v.z;
		float tmpZ = -s * v.y + c * v.z;
		return new Vector4 (v.x, tmpY, tmpZ, v.w);
	}

	Vector4 RotateAroundYW (Vector4 v, float s, float c)
	{
		float tmpY = c * v.y - s * v.w;
		float tmpW = s * v.y + c * v.w;
		return new Vector4 (v.x, tmpY, v.z, tmpW);
	}

	Vector4 RotateAroundZW (Vector4 v, float s, float c)
	{
		float tmpZ = c * v.z - s * v.w;
		float tmpW = s * v.z + c * v.w;
		return new Vector4 (v.x, v.y, tmpZ, tmpW);
	}

	void Rotate (Axis4D axis, float theta) // Creates our Rotate function, that requires an axis and a float (theta)
	{
		AddToRotationDictionary (axis, theta); // Calls our AddToRotationDictionary function
		ApplyRotationToVerts (); // Calls our ApplyRotationToVerts Function
	}

	void AddToRotationDictionary (Axis4D axis, float theta) // Creates our function, requires an axis and a float
	{
		rotation [axis] = (rotation [axis] + theta); // Makes the rotation value for our axis = to the axis plus our theta float

	}

	void ApplyRotationToVerts () // Creates our function
	{
		ResetVertices (); // Calls the Reset Vertices's function

		foreach (Axis4D axis in rotationOrder) { // Runs a set of calculations on every element in our rotationOrder list that contains an Axis4d axis
			float s = Mathf.Sin (Mathf.Deg2Rad * rotation [axis]); // Creates a variable s and sets it to the sine of the radian value of the element in the rotation list
			float c = Mathf.Cos (Mathf.Deg2Rad * rotation [axis]); // Creates a variable c and sets it to the cosine of the radian value of the element in the rotation list
			for (int i = 0; i < rotatedVerts.Count; i++) {
				rotatedVerts [i] = GetRotatedVertex (axis, rotatedVerts [i], s, c);
			}
		}
	}

	void ResetVertices () // Creates the function ResetVertice and makes the value of the displayed vertices's = the list of the original vertices's
	{
		rotatedVerts = new List<Vector4> ();
		rotatedVerts.AddRange (originalVerts);
	}

	void OnDrawGizmosSelected () // Draws circle gizmo's (only display in editor when object is selected), and sets there position to the value of the rotated vertices's list
	{
		Gizmos.color = Color.yellow;
		for (int i = 0; i < rotatedVerts.Count; i++) {
			Gizmos.DrawSphere (rotatedVerts [i], 0.1f);
		}


	}
}