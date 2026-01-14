using DTO;
using UnityEngine;
using System.Collections.Generic;

namespace Interface.ConstellationInfo
{
    public class ConstellationData : MonoBehaviour
    {
        public ConstellationDto Constellation;
        
        public void Init(ConstellationDto constellation)
        {
            Constellation = constellation;
        }
        
        public string ConstellationName => Constellation.name;
        public ConstellationStarBlockDto Stars => Constellation.stars;

        // --- Highlight support ---
        private List<Renderer> _renderers;
        private List<Color> _originalRendererColors;

        private List<LineRenderer> _lineRenderers;
        private List<Color> _originalLineStartColors;
        private List<Color> _originalLineEndColors;

        private bool _isHighlighted = false;

        /// <summary>
        /// Change la couleur des renderers et line renderers pour mettre en évidence cette constellation.
        /// Stocke les couleurs originales pour pouvoir les restaurer.
        /// </summary>
        public void Highlight(Color color)
        {
            if (_isHighlighted) return;

            // Collecte des Renderers (étoiles)
            _renderers = new List<Renderer>();
            _originalRendererColors = new List<Color>();

            foreach (Renderer r in GetComponentsInChildren<Renderer>(true))
            {
                // Ignorer les Renderers qui sont des UI ou non visibles si nécessaire, mais pour l'instant on sauvegarde tous
                _renderers.Add(r);
                if (r.material != null && r.material.HasProperty("_Color"))
                    _originalRendererColors.Add(r.material.color);
                else
                    _originalRendererColors.Add(Color.white);

                if (r.material != null && r.material.HasProperty("_Color"))
                    r.material.color = color;
            }

            // Collecte des LineRenderers (segments de la constellation)
            _lineRenderers = new List<LineRenderer>();
            _originalLineStartColors = new List<Color>();
            _originalLineEndColors = new List<Color>();

            foreach (LineRenderer lr in GetComponentsInChildren<LineRenderer>(true))
            {
                _lineRenderers.Add(lr);
                _originalLineStartColors.Add(lr.startColor);
                _originalLineEndColors.Add(lr.endColor);

                lr.startColor = color;
                lr.endColor = color;

                if (lr.material != null && lr.material.HasProperty("_Color"))
                    lr.material.color = color;
            }

            _isHighlighted = true;
        }

        /// <summary>
        /// Restaure les couleurs originales des composants modifiés par Highlight.
        /// </summary>
        public void Unhighlight()
        {
            if (!_isHighlighted) return;

            if (_renderers != null)
            {
                for (int i = 0; i < _renderers.Count; i++)
                {
                    var r = _renderers[i];
                    if (r == null) continue;
                    if (r.material != null && r.material.HasProperty("_Color"))
                    {
                        r.material.color = _originalRendererColors[i];
                    }
                }
            }

            if (_lineRenderers != null)
            {
                for (int i = 0; i < _lineRenderers.Count; i++)
                {
                    var lr = _lineRenderers[i];
                    if (lr == null) continue;
                    lr.startColor = _originalLineStartColors[i];
                    lr.endColor = _originalLineEndColors[i];
                    if (lr.material != null && lr.material.HasProperty("_Color"))
                        lr.material.color = _originalLineStartColors[i];
                }
            }

            // Clear cached lists
            _renderers = null;
            _originalRendererColors = null;
            _lineRenderers = null;
            _originalLineStartColors = null;
            _originalLineEndColors = null;

            _isHighlighted = false;
        }
    }
}