using UnityEngine;

[CreateAssetMenu(fileName = "BuildingMaterials", menuName = "Scriptable Objects/BuildingMaterials")]
public class BuildingMaterials : ScriptableObject
{
    public Material placingMaterialUnallowed;
    public Material placingMaterialAllowed;
    public Material builtMaterial;
    public Material constructingMaterial;
}
