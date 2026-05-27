using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

namespace GhostSignal.UI
{
    public class LevelCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        [Header("Referencias de Componentes de UI")]
        [SerializeField] private Image imgBorder;
        [SerializeField] private Image imgThumbnail;
        [SerializeField] private GameObject objGlowShadow;
        
        [Header("Etiquetas de Texto (TMP)")]
        [SerializeField] private TMP_Text txtNombre;
        [SerializeField] private TMP_Text txtDescripcion;
        [SerializeField] private TMP_Text txtStatTiempo;
        [SerializeField] private TMP_Text txtStatFragmentos;
        
        [Header("Pill de Estado")]
        [SerializeField] private TMP_Text txtPillEstado;
        [SerializeField] private Image imgPillBg;

        public LevelData Data { get; private set; }
        public Button CardButton { get; private set; }
        
        public static event Action<LevelData> OnCardFocused;
        public static event Action<LevelData> OnCardSelected;

        private Vector3 _initialScale;
        private bool _isLocked;

        private void Awake()
        {
            CardButton = GetComponent<Button>();
            _initialScale = transform.localScale;
            if (objGlowShadow) objGlowShadow.SetActive(false);
        }

        public void Bind(LevelData data)
        {
            Data = data;
            _initialScale = Vector3.one;
            transform.localScale = _initialScale;

            txtNombre.text = Data.displayName;
            txtDescripcion.text = Data.description;
            
            LevelData.LevelStatus status = LevelProgress.GetStatus(Data);
            _isLocked = (status == LevelData.LevelStatus.Locked);
            
            ConfigurarPill(status);
            ConfigurarStats();
            AplicarEsteticaBase(status);
        }

        private void ConfigurarPill(LevelData.LevelStatus status)
        {
            switch (status)
            {
                case LevelData.LevelStatus.Locked:
                    txtPillEstado.text = "BLOQUEADO";
                    txtPillEstado.color = new Color(1f, 1f, 1f, 0.55f);
                    imgPillBg.color = new Color(0.2f, 0.2f, 0.2f, 0.55f);
                    if (CardButton) CardButton.interactable = false;
                    break;
                case LevelData.LevelStatus.Available:
                    txtPillEstado.text = "DISPONIBLE";
                    txtPillEstado.color = Data.accentColor;
                    imgPillBg.color = new Color(0f, 0f, 0f, 0.7f);
                    if (CardButton) CardButton.interactable = true;
                    break;
                case LevelData.LevelStatus.Current:
                    txtPillEstado.text = "EN CURSO";
                    txtPillEstado.color = new Color(0.96f, 0.78f, 0.29f); // Ámbar
                    imgPillBg.color = new Color(0.96f, 0.78f, 0.29f, 0.15f);
                    if (CardButton) CardButton.interactable = true;
                    break;
                case LevelData.LevelStatus.Completed:
                    txtPillEstado.text = "COMPLETO ✓";
                    txtPillEstado.color = new Color(0.3f, 1f, 0.54f); // Verde OK
                    imgPillBg.color = new Color(0.3f, 1f, 0.54f, 0.15f);
                    if (CardButton) CardButton.interactable = true;
                    break;
            }
        }

        private void ConfigurarStats()
        {
            float bestTime = LevelProgress.GetBestTime(Data);
            if (bestTime > 0f)
            {
                int mins = Mathf.FloorToInt(bestTime / 60f);
                int secs = Mathf.FloorToInt(bestTime % 60f);
                txtStatTiempo.text = $"TIME: {mins:D2}:{secs:D2}";
            }
            else
            {
                txtStatTiempo.text = "TIME: --:--";
            }

            int collected = LevelProgress.GetFragments(Data);
            txtStatFragmentos.text = $"FRAG: {collected:D2}/{Data.fragmentsTotal:D2}";
        }

        private void AplicarEsteticaBase(LevelData.LevelStatus status)
        {
            if (imgThumbnail && Data.thumbnail) imgThumbnail.sprite = Data.thumbnail;
            
            if (status == LevelData.LevelStatus.Locked)
            {
                if (imgThumbnail) imgThumbnail.color = new Color(0.2f, 0.2f, 0.2f, 0.55f);
                txtNombre.color = new Color(1f, 1f, 1f, 0.3f);
                if (imgBorder) imgBorder.color = new Color(0.17f, 0.88f, 0.9f, 0.15f);
            }
            else
            {
                if (imgThumbnail) imgThumbnail.color = Color.white;
                txtNombre.color = Data.accentColor;
                if (imgBorder) imgBorder.color = new Color(0.17f, 0.88f, 0.9f, 0.3f); // Cian base opacidad 0.3
            }
            
            if (CardButton)
            {
                CardButton.onClick.RemoveAllListeners();
                CardButton.onClick.AddListener(() => OnCardSelected?.Invoke(Data));
            }
        }

        public void Highlight(bool enable)
        {
            if (_isLocked) return;

            if (enable)
            {
                transform.localScale = _initialScale * 1.03f;
                if (imgBorder) imgBorder.color = Data.accentColor;
                if (objGlowShadow) objGlowShadow.SetActive(true);
                OnCardFocused?.Invoke(Data);
            }
            else
            {
                transform.localScale = _initialScale;
                if (imgBorder) imgBorder.color = new Color(0.17f, 0.88f, 0.9f, 0.3f);
                if (objGlowShadow) objGlowShadow.SetActive(false);
            }
        }

        public void OnPointerEnter(PointerEventData eventData) { if (!_isLocked && CardButton) CardButton.Select(); }
        public void OnPointerExit(PointerEventData eventData) { }
        public void OnSelect(BaseEventData eventData) { Highlight(true); }
        public void OnDeselect(BaseEventData eventData) { Highlight(false); }
    }
}