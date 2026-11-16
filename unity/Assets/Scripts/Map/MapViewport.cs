// MARSEILLE — Map/MapViewport.cs
// Déplacement et zoom de la carte : glisser d'un doigt, pincer à deux doigts, molette en éditeur.
// Posé sur la zone d'affichage (avec un masque) ; déplace et met à l'échelle le contenu.

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Marseille.Map
{
    /// <summary>Navigation dans la carte.</summary>
    [RequireComponent(typeof(RectTransform))]
    public class MapViewport : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
    {
        [SerializeField] private RectTransform content;

        private Vector2 zoomRange = new Vector2(0.6f, 3f);
        private float scrollStep = 0.1f;
        private float zoom = 1f;
        private float lastPinchDistance;
        private bool pinching;

        /// <summary>Le zoom a changé (nouvelle valeur).</summary>
        public event Action<float> ZoomChanged;

        /// <summary>Zoom actuel.</summary>
        public float Zoom { get { return zoom; } }

        /// <summary>Contenu déplacé (le plan).</summary>
        public RectTransform Content { get { return content; } set { content = value; } }

        /// <summary>Garantit une surface qui reçoit les glissements et un masque qui coupe le plan aux bords.</summary>
        private void Awake()
        {
            if (GetComponent<Graphic>() == null)
            {
                Image catcher = gameObject.AddComponent<Image>();
                catcher.color = new Color(0f, 0f, 0f, 0f);
            }
            if (GetComponent<RectMask2D>() == null) gameObject.AddComponent<RectMask2D>();
        }

        /// <summary>Règle les limites et le zoom de départ.</summary>
        /// <param name="range">Zoom minimal et maximal.</param>
        /// <param name="start">Zoom de départ.</param>
        /// <param name="step">Pas de la molette.</param>
        public void Configure(Vector2 range, float start, float step)
        {
            zoomRange = range;
            scrollStep = step;
            SetZoom(start);
        }

        /// <summary>Début de glissement.</summary>
        /// <param name="eventData">Données.</param>
        public void OnBeginDrag(PointerEventData eventData) { }

        /// <summary>Glissement d'un doigt : déplace le plan (désactivé pendant un pincement).</summary>
        /// <param name="eventData">Données.</param>
        public void OnDrag(PointerEventData eventData)
        {
            if (content == null || pinching || Input.touchCount > 1) return;
            Canvas canvas = GetComponentInParent<Canvas>();
            float scale = canvas != null ? canvas.scaleFactor : 1f;
            content.anchoredPosition += eventData.delta / scale;
            Clamp();
        }

        /// <summary>Fin de glissement.</summary>
        /// <param name="eventData">Données.</param>
        public void OnEndDrag(PointerEventData eventData) { }

        /// <summary>Molette : zoom.</summary>
        /// <param name="eventData">Données.</param>
        public void OnScroll(PointerEventData eventData)
        {
            SetZoom(zoom + eventData.scrollDelta.y * scrollStep);
        }

        /// <summary>Pincement à deux doigts (lu seulement quand deux doigts touchent l'écran).</summary>
        private void Update()
        {
            if (Input.touchCount != 2) { pinching = false; return; }
            Touch a = Input.GetTouch(0), b = Input.GetTouch(1);
            float distance = Vector2.Distance(a.position, b.position);
            if (!pinching) { pinching = true; lastPinchDistance = distance; return; }
            if (lastPinchDistance > 1f) SetZoom(zoom * distance / lastPinchDistance);
            lastPinchDistance = distance;
        }

        /// <summary>Applique un zoom borné.</summary>
        /// <param name="value">Zoom voulu.</param>
        public void SetZoom(float value)
        {
            float z = Mathf.Clamp(value, zoomRange.x, zoomRange.y);
            if (content == null) { zoom = z; return; }
            content.anchoredPosition *= z / Mathf.Max(0.0001f, zoom);
            zoom = z;
            content.localScale = new Vector3(z, z, 1f);
            Clamp();
            if (ZoomChanged != null) ZoomChanged(zoom);
        }

        /// <summary>Centre la vue sur un point du plan.</summary>
        /// <param name="planPosition">Position dans le plan (non mise à l'échelle).</param>
        public void CenterOn(Vector2 planPosition)
        {
            if (content == null) return;
            content.anchoredPosition = -planPosition * zoom;
            Clamp();
        }

        /// <summary>Empêche le plan de sortir entièrement de la zone d'affichage.</summary>
        private void Clamp()
        {
            RectTransform view = (RectTransform)transform;
            Vector2 half = (content.rect.size * zoom - view.rect.size) * 0.5f;
            half = Vector2.Max(half, Vector2.zero);
            Vector2 p = content.anchoredPosition;
            content.anchoredPosition = new Vector2(Mathf.Clamp(p.x, -half.x, half.x), Mathf.Clamp(p.y, -half.y, half.y));
        }
    }
}
