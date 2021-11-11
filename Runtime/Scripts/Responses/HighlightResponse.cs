using System.Collections.Generic;
using UnityEngine;

namespace ExpressoBits.Interactions
{
    public class HighlightResponse : MonoBehaviour, ISelectionResponse
    {

        [SerializeField] private Material[] highlightMaterials;

        private Dictionary<Renderer,Material[]> selectionMaterials = new Dictionary<Renderer, Material[]>();

        public void OnSelect(IInteractable interactable)
        {
            ChangeMaterials(interactable.Transform);
        }

        public void OnDeselect(IInteractable interactable)
        {
            RestoreMaterials(interactable.Transform);
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


