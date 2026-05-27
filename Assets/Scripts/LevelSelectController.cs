using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

namespace GhostSignal.UI
{
    public class LevelSelectController : MonoBehaviour
    {
        [Header("Datos de Entrada")]
        [SerializeField] private LevelData[] levels;
        [SerializeField] private GameObject cardPrefab;
        
        [Header("Contenedores de Estructura")]
        [SerializeField] private Transform contenedorNiveles;
        [SerializeField] private GameObject menuInicioRef;

        [Header("Botones del Footer")]
        [SerializeField] private Button btnIniciar;
        [SerializeField] private Button btnTutorial;

        [Header("HUD de Información Central")]
        [SerializeField] private TMP_Text txtConexiones;
        [SerializeField] private TMP_Text txtFragmentos;
        [SerializeField] private TMP_Text txtMejorTiempo;
        [SerializeField] private TMP_Text txtHint;

        private LevelData _focusedLevel;
        private LevelCard[] _instantiatedCards;

        private void OnEnable()
        {
            LevelCard.OnCardFocused += HandleCardFocused;
            LevelCard.OnCardSelected += HandleCardSelected;
        }

        private void OnDisable()
        {
            LevelCard.OnCardFocused -= HandleCardFocused;
            LevelCard.OnCardSelected -= HandleCardSelected;
        }

        private void Start()
        {
            ConstruirPanel();
            ConfigurarNavegacionEstatica();
            CalcularMetricasGlobales();
        }

        private void ConstruirPanel()
        {
            // Limpieza preventiva del contenedor
            foreach (Transform child in contenedorNiveles)
            {
                Destroy(child.gameObject);
            }

            _instantiatedCards = new LevelCard[levels.Length];

            for (int i = 0; i < levels.Length; i++)
            {
                GameObject go = Instantiate(cardPrefab, contenedorNiveles);
                LevelCard card = go.GetComponent<LevelCard>();
                card.Bind(levels[i]);
                _instantiatedCards[i] = card;
            }

            // Forzar foco inicial en la primera tarjeta disponible
            StartCoroutine(ForzarFocoInicial());
        }

        private IEnumerator ForzarFocoInicial()
        {
            yield return new WaitForEndOfFrame();
            if (_instantiatedCards.Length > 0)
            {
                for (int i = 0; i < _instantiatedCards.Length; i++)
                {
                    if (_instantiatedCards[i].CardButton && _instantiatedCards[i].CardButton.interactable)
                    {
                        _instantiatedCards[i].CardButton.Select();
                        break;
                    }
                }
            }
        }

        private void ConfigurarNavegacionEstatica()
        {
            if (btnTutorial) btnTutorial.onClick.AddListener(VolverAlTutorial);
            if (btnIniciar) btnIniciar.onClick.AddListener(() => { if (_focusedLevel != null) HandleCardSelected(_focusedLevel); });
        }

        private void Update()
        {
            // Atajos directos de la terminal
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                VolverAlMenuInicio();
            }
        }

        private void HandleCardFocused(LevelData data)
        {
            _focusedLevel = data;
            
            // Actualización de métricas en tiempo real en el Header
            float bestTime = LevelProgress.GetBestTime(data);
            if (txtMejorTiempo)
            {
                if (bestTime > 0f)
                {
                    int mins = Mathf.FloorToInt(bestTime / 60f);
                    int secs = Mathf.FloorToInt(bestTime % 60f);
                    txtMejorTiempo.text = $"MEJOR T. {mins:D2}:{secs:D2}";
                }
                else
                {
                    txtMejorTiempo.text = "MEJOR T. --:--";
                }
            }

            // Cambiar dinámicamente la estética de confirmación del botón de acción
            if (btnIniciar)
            {
                btnIniciar.interactable = true;
                var txtBtn = btnIniciar.GetComponentInChildren<TMP_Text>();
                if (txtBtn != null)
                {
                    txtBtn.text = $"[ENTER] INFILTRAR {data.displayName} ►";
                    txtBtn.color = data.accentColor;
                }
            }
        }

        private void HandleCardSelected(LevelData data)
        {
            if (LevelProgress.GetStatus(data) == LevelData.LevelStatus.Locked) return;
            
            // Simulación de carga / Ejecución de cambio de escena URP
            if (!string.IsNullOrEmpty(data.sceneName))
            {
                SceneManager.LoadScene(data.sceneName);
            }
            else
            {
                Debug.LogWarning($"El nivel {data.displayName} no tiene una escena asignada en LevelData.");
            }
        }

        private void CalcularMetricasGlobales()
        {
            int completados = 0;
            int fragmentosTotalesRecolectados = 0;
            int fragmentosMaximosPosibles = 0;

            foreach (var lvl in levels)
            {
                if (LevelProgress.GetStatus(lvl) == LevelData.LevelStatus.Completed) completados++;
                fragmentosTotalesRecolectados += LevelProgress.GetFragments(lvl);
                fragmentosMaximosPosibles += lvl.fragmentsTotal;
            }

            if (txtConexiones) txtConexiones.text = $"CONEXIONES {completados}/{levels.Length}";
            if (txtFragmentos) txtFragmentos.text = $"FRAGMENTOS {fragmentosTotalesRecolectados}/{fragmentosMaximosPosibles}";
        }

        private void VolverAlTutorial()
        {
            SceneManager.LoadScene("Level0_Tutorial");
        }

        private void VolverAlMenuInicio()
        {
            if (menuInicioRef != null)
            {
                menuInicioRef.SetActive(true);
                gameObject.SetActive(false); // Apagar Panel_Niveles
            }
        }
    }
}