using System.Collections.Generic;
using UnityEngine;

namespace ExpressoBits.Interactions
{
    [CreateAssetMenu(fileName = "Highlight Selection Response",menuName = "Expresso Bits/Interactions/Highlight Selection Response")]
    public class HighlightResponse : SelectionResponse
    {

        [SerializeField] private Material[] highlightMaterials;
        private Dictionary<Renderer,Material[]> selectionMaterials = new Dictionary<Renderer, Material[]>();

        public override void OnSelect(Transform interactable)
        {
            ChangeMaterials(interactable);
        }

        public override void OnDeselect(Transform interactable)
        {
            RestoreMaterials(interactable);
        }

        private void RestoreMaterials(Transform selection)
        {
            Renderer[] renderers = selection.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.materials = selectionMaterials[renderer];
            }
        }

        private void ChangeMaterials(Transform selection)
        {
            if(highlightMaterials.Length <= 0) return;
            selectionMaterials = new Dictionary<Renderer, Material[]>();

            Renderer[] renderers = selection.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                Material[] materials = renderer.materials;
                selectionMaterials.Add(renderer,materials);

                Material[] highlightMaterials = ChangeHighlightMaterialsRenderer(materials);
                renderer.materials = highlightMaterials;
            }
        }

        private Material[] ChangeHighlightMaterialsRenderer(Material[] materials)
        {
            // TODO destroy materials instances in garbage see: https://docs.unity3d.com/ScriptReference/Renderer-materials.html
            Material[] newMaterials = new Material[materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                newMaterials[i] = GetHighlightMaterial(i);
            }
            return newMaterials;
        }

        private Material GetHighlightMaterial(int index)
        {
            return highlightMaterials[index % highlightMaterials.Length];
        }
    

    }
}


