//
//  Outline.cs
//  QuickOutline
//
//  Created by Chris Nolet on 3/30/18.
//  Copyright © 2018 Chris Nolet. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]

//<summary
// Online package avaiable by author Chris Nolet to outline gameObjects in multiple ways.
//</summary>/
public class Outline : MonoBehaviour {

  // keeps track of meshes that have already had their smooth normals registered
  private static HashSet<Mesh> registeredMeshes = new HashSet<Mesh>();

  /// <summary>
  /// Defines the different modes for how the outline effect is rendered.
  /// </summary>
  public enum Mode {
    OutlineAll, // outlines all parts of object, regardless of visibility
    OutlineVisible, // outlines parts of object visible to camera
    OutlineHidden, // outlines parts of object hidden behind other objects
    OutlineAndSilhouette, // outlines visible parts and renders silhouette of object
    SilhouetteOnly // renders only silhouette of object
  }

  /// <summary>
  /// Gets or sets current outline mode.
  /// </summary>
  public Mode OutlineMode {
    get { return outlineMode; }
    set {
      outlineMode = value;
      needsUpdate = true;
    }
  }

  /// <summary>
  /// Gets or sets color of the outline.
  /// </summary>
  public Color OutlineColor {
    get { return outlineColor; }
    set {
      outlineColor = value;
      needsUpdate = true;
    }
  }

  /// <summary>
  /// Gets or sets thickness of the outline.
  /// </summary>
  public float OutlineWidth {
    get { return outlineWidth; }
    set {
      outlineWidth = value;
      needsUpdate = true;
    }
  }

  // Serializable class used for internally serializing precomputed smooth normals.
  [Serializable]
  private class ListVector3 {
    public List<Vector3> data;
  }

  /// <summary> Current mode of outline effect. </summary>
  [SerializeField]
  private Mode outlineMode;

  /// <summary> Color of the outline. </summary>
  [SerializeField]
  private Color outlineColor = Color.white;

  /// <summary> Width of the outline. </summary>
  [SerializeField, Range(0f, 10f)]
  private float outlineWidth = 2f;

  [Header("Optional")]

  /// <summary> Enables precomputation of smooth normals. </summary>
  [SerializeField, Tooltip("Precompute enabled: Per-vertex calculations are performed in the editor and serialized with the object. "
  + "Precompute disabled: Per-vertex calculations are performed at runtime in Awake(). This may cause a pause for large meshes.")]
  private bool precomputeOutline;

  /// <summary>
  /// List of Mesh assets used as keys for storing precomputed smooth normals.
  /// </summary>
  [SerializeField, HideInInspector]
  private List<Mesh> bakeKeys = new List<Mesh>();

  /// <summary>
  /// A list of ListVector3 objects storing the precomputed smooth normals corresponding to the bakeKeys.
  /// </summary>
  [SerializeField, HideInInspector]
  private List<ListVector3> bakeValues = new List<ListVector3>();

  private Renderer[] renderers; // array of Renderer components in GameObject
  private Material outlineMaskMaterial; // Material instance used to create outline mask effect
  private Material outlineFillMaterial; // Material instance used to fill the outline with specified color

  private bool needsUpdate;

  /// <summary>
  /// Called when script instance is loaded.
  /// Caches renderers, instantiates outline materials, and loads smooth normals.
  /// </summary>
  void Awake() {

    // Cache renderers
    renderers = GetComponentsInChildren<Renderer>();

    // Instantiate outline materials
    outlineMaskMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineMask"));
    outlineFillMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineFill"));

    outlineMaskMaterial.name = "OutlineMask (Instance)";
    outlineFillMaterial.name = "OutlineFill (Instance)";

    // Retrieve or generate smooth normals
    LoadSmoothNormals();

    // Apply material properties immediately
    needsUpdate = true;
  }

  /// <summary>
  /// Called when component becomes enabled.
  /// Appends the outline mask and fill materials to the materials of each renderer.
  /// </summary>
  void OnEnable() {
    foreach (var renderer in renderers) {

      // Append outline shaders
      var materials = renderer.sharedMaterials.ToList();

      materials.Add(outlineMaskMaterial);
      materials.Add(outlineFillMaterial);

      renderer.materials = materials.ToArray();
    }
  }

  /// <summary>
  /// Called in editor when script is loaded or value is changed in Inspector.
  /// Updates material properties, clears the bake cache if precomputation is disabled or corrupted,
  /// and triggers the baking of smooth normals if precomputation is enabled and no data exists.
  /// </summary>
  void OnValidate() {

    // Update material properties
    needsUpdate = true;

    // Clear cache when baking is disabled or corrupted
    if (!precomputeOutline && bakeKeys.Count != 0 || bakeKeys.Count != bakeValues.Count) {
      bakeKeys.Clear();
      bakeValues.Clear();
    }

    // Generate smooth normals when baking is enabled
    if (precomputeOutline && bakeKeys.Count == 0) {
      Bake();
    }
  }

  /// <summary>
  /// Applies material properties if an update is needed.
  /// </summary>
  void Update() {
    if (needsUpdate) {
      needsUpdate = false;

      UpdateMaterialProperties();
    }
  }

  /// <summary>
  /// Called when component is disabled.
  /// Removes instantiated outline and fill materials.
  /// </summary>
  void OnDisable() {
    foreach (var renderer in renderers) {

      // Remove outline shaders
      var materials = renderer.sharedMaterials.ToList();

      materials.Remove(outlineMaskMaterial);
      materials.Remove(outlineFillMaterial);

      renderer.materials = materials.ToArray();
    }
  }

  /// <summary>
  /// Destroys instantiated outline and fill materials to prevent memory leaks.
  /// </summary>
  void OnDestroy() {

    // Destroy material instances
    Destroy(outlineMaskMaterial);
    Destroy(outlineFillMaterial);
  }

  /// <summary>
  /// Generates and serializes smooth normals for all MeshFilter components in the GameObject and its children.
  /// </summary>
  void Bake() {

    // Generate smooth normals for each mesh
    var bakedMeshes = new HashSet<Mesh>();

    foreach (var meshFilter in GetComponentsInChildren<MeshFilter>()) {

      // Skip duplicates
      if (!bakedMeshes.Add(meshFilter.sharedMesh)) {
        continue;
      }

      // Serialize smooth normals
      var smoothNormals = SmoothNormals(meshFilter.sharedMesh);

      bakeKeys.Add(meshFilter.sharedMesh);
      bakeValues.Add(new ListVector3() { data = smoothNormals });
    }
  }

  /// <summary>
  /// Loads precomputed smooth normals from the `bakeValues` list or generates them at runtime
  /// for all MeshFilter and SkinnedMeshRenderer components in the GameObject and its children.
  /// </summary>
  void LoadSmoothNormals() {

    // Retrieve or generate smooth normals
    foreach (var meshFilter in GetComponentsInChildren<MeshFilter>()) {

      // Skip if smooth normals have already been adopted
      if (!registeredMeshes.Add(meshFilter.sharedMesh)) {
        continue;
      }

      // Retrieve or generate smooth normals
      var index = bakeKeys.IndexOf(meshFilter.sharedMesh);
      var smoothNormals = (index >= 0) ? bakeValues[index].data : SmoothNormals(meshFilter.sharedMesh);

      // Store smooth normals in UV3
      meshFilter.sharedMesh.SetUVs(3, smoothNormals);

      // Combine submeshes
      var renderer = meshFilter.GetComponent<Renderer>();

      if (renderer != null) {
        CombineSubmeshes(meshFilter.sharedMesh, renderer.sharedMaterials);
      }
    }

    // Clear UV3 on skinned mesh renderers
    foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>()) {

      // Skip if UV3 has already been reset
      if (!registeredMeshes.Add(skinnedMeshRenderer.sharedMesh)) {
        continue;
      }

      // Clear UV3
      skinnedMeshRenderer.sharedMesh.uv4 = new Vector2[skinnedMeshRenderer.sharedMesh.vertexCount];

      // Combine submeshes
      CombineSubmeshes(skinnedMeshRenderer.sharedMesh, skinnedMeshRenderer.sharedMaterials);
    }
  }

  /// <summary>
  /// Calculates smooth normals for a given mesh by averaging the normals of vertices that share the same position.
  /// </summary>
  /// <param name="mesh"></param>
  /// <returns> A List of Vector3 representing the smooth normals for each vertex of the mesh. </returns>
  List<Vector3> SmoothNormals(Mesh mesh) {

    // Group vertices by location
    var groups = mesh.vertices.Select((vertex, index) => new KeyValuePair<Vector3, int>(vertex, index)).GroupBy(pair => pair.Key);

    // Copy normals to a new list
    var smoothNormals = new List<Vector3>(mesh.normals);

    // Average normals for grouped vertices
    foreach (var group in groups) {

      // Skip single vertices
      if (group.Count() == 1) {
        continue;
      }

      // Calculate the average normal
      var smoothNormal = Vector3.zero;

      foreach (var pair in group) {
        smoothNormal += smoothNormals[pair.Value];
      }

      smoothNormal.Normalize();

      // Assign smooth normal to each vertex
      foreach (var pair in group) {
        smoothNormals[pair.Value] = smoothNormal;
      }
    }

    return smoothNormals;
  }

  /// <summary>
  /// Combines all submeshes of a given mesh into a single submesh.
  /// Ensures the outline shader works correctly on objects with multiple materials/submeshes.
  /// </summary>
  /// <param name="mesh"></param>
  /// <param name="materials"></param>
  void CombineSubmeshes(Mesh mesh, Material[] materials) {

    // Skip meshes with a single submesh
    if (mesh.subMeshCount == 1) {
      return;
    }

    // Skip if submesh count exceeds material count
    if (mesh.subMeshCount > materials.Length) {
      return;
    }

    // Append combined submesh
    mesh.subMeshCount++;
    mesh.SetTriangles(mesh.triangles, mesh.subMeshCount - 1);
  }

  /// <summary>
  /// Updates the properties of the outline materials based on the current `OutlineMode`, `OutlineColor`, and `OutlineWidth`.
  /// This method sets the `_OutlineColor` on the `outlineFillMaterial` and adjusts the `_ZTest` and `_OutlineWidth`
  /// properties on both the `outlineMaskMaterial` and `outlineFillMaterial` according to the selected outline mode.
  /// </summary>
  void UpdateMaterialProperties() {

    // Apply properties according to mode
    outlineFillMaterial.SetColor("_OutlineColor", outlineColor);

    switch (outlineMode) {
      case Mode.OutlineAll:
        outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
        outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
        outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
        break;

      case Mode.OutlineVisible:
        outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
        outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
        outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
        break;

      case Mode.OutlineHidden:
        outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
        outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
        outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
        break;

      case Mode.OutlineAndSilhouette:
        outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
        outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
        outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
        break;

      case Mode.SilhouetteOnly:
        outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
        outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
        outlineFillMaterial.SetFloat("_OutlineWidth", 0f);
        break;
    }
  }
}
