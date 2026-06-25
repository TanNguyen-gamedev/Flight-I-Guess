#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FlightIGuess.Shop.Unity;

namespace FlightIGuess.Editor
{
    public class ShopUIGenerator : MonoBehaviour
    {
        [MenuItem("Tools/FlightIGuess/Generate Shop UI")]
        public static void GenerateShopUI()
        {
            // 1. Create Canvas
            GameObject canvasGO = new GameObject("ShopCanvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGO.AddComponent<GraphicRaycaster>();
            
            ShopPresenter shopPresenter = canvasGO.AddComponent<ShopPresenter>();

            // 2. Create Shop Panel (Background)
            GameObject shopPanel = CreateUIElement("ShopPanel", canvasGO.transform);
            Image panelImage = shopPanel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.8f);
            SetRectTransform(shopPanel, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one);
            
            VerticalLayoutGroup mainLayout = shopPanel.AddComponent<VerticalLayoutGroup>();
            mainLayout.padding = new RectOffset(50, 50, 50, 50);
            mainLayout.spacing = 20;
            mainLayout.childControlHeight = true;
            mainLayout.childControlWidth = true;
            mainLayout.childForceExpandHeight = false;

            // 3. Top Bar
            GameObject topBar = CreateUIElement("TopBar", shopPanel.transform);
            HorizontalLayoutGroup topBarLayout = topBar.AddComponent<HorizontalLayoutGroup>();
            topBarLayout.childControlHeight = true;
            topBarLayout.childControlWidth = false;
            topBarLayout.childForceExpandWidth = false;
            topBarLayout.spacing = 50;

            GameObject titleGO = CreateTextElement("ShopTitle", topBar.transform, "SHOP", 48);
            
            GameObject scrapInfoGO = CreateUIElement("ScrapInfo", topBar.transform);
            HorizontalLayoutGroup scrapLayout = scrapInfoGO.AddComponent<HorizontalLayoutGroup>();
            scrapLayout.spacing = 10;
            GameObject scrapTextGO = CreateTextElement("ScrapAmountText", scrapInfoGO.transform, "Scrap: 0", 36);

            // 4. Middle Content (Stats + Items)
            GameObject middleContent = CreateUIElement("MiddleContent", shopPanel.transform);
            HorizontalLayoutGroup middleLayout = middleContent.AddComponent<HorizontalLayoutGroup>();
            middleLayout.spacing = 50;
            middleLayout.childForceExpandHeight = true;
            
            // LayoutElement to make middle content take up remaining space
            LayoutElement middleLE = middleContent.AddComponent<LayoutElement>();
            middleLE.flexibleHeight = 1;

            // 4a. Left Panel (Stats)
            GameObject statsPanel = CreateUIElement("StatsPanel", middleContent.transform);
            Image statsBg = statsPanel.AddComponent<Image>();
            statsBg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            VerticalLayoutGroup statsLayout = statsPanel.AddComponent<VerticalLayoutGroup>();
            statsLayout.padding = new RectOffset(20, 20, 20, 20);
            statsLayout.spacing = 15;
            LayoutElement statsLE = statsPanel.AddComponent<LayoutElement>();
            statsLE.preferredWidth = 300;

            CreateTextElement("StatsTitle", statsPanel.transform, "Ship Stats", 32);
            CreateTextElement("HPText", statsPanel.transform, "Max HP: --", 24);
            CreateTextElement("ThrustText", statsPanel.transform, "Thrust: --", 24);
            CreateTextElement("TurnText", statsPanel.transform, "Turn Rate: --", 24);

            // 4b. Right Panel (Items Grid)
            GameObject itemsPanel = CreateUIElement("ItemsPanel", middleContent.transform);
            GridLayoutGroup gridLayout = itemsPanel.AddComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(250, 350);
            gridLayout.spacing = new Vector2(20, 20);
            LayoutElement itemsLE = itemsPanel.AddComponent<LayoutElement>();
            itemsLE.flexibleWidth = 1;

            // 5. Bottom Bar
            GameObject bottomBar = CreateUIElement("BottomBar", shopPanel.transform);
            HorizontalLayoutGroup bottomLayout = bottomBar.AddComponent<HorizontalLayoutGroup>();
            bottomLayout.childAlignment = TextAnchor.MiddleRight;
            
            GameObject rerollButtonGO = CreateButtonElement("RerollButton", bottomBar.transform, "Reroll (10)");
            LayoutElement rerollLE = rerollButtonGO.AddComponent<LayoutElement>();
            rerollLE.preferredWidth = 200;
            rerollLE.preferredHeight = 60;

            // 6. Create Prefab for ShopItemUI
            GameObject itemPrefabGO = CreateShopItemPrefab();

            // Link up the Presenter
            SerializedObject serializedPresenter = new SerializedObject(shopPresenter);
            serializedPresenter.FindProperty("_shopCanvas").objectReferenceValue = canvas;
            serializedPresenter.FindProperty("_shopItemContainer").objectReferenceValue = itemsPanel.transform;
            serializedPresenter.FindProperty("_rerollButton").objectReferenceValue = rerollButtonGO.GetComponent<Button>();
            serializedPresenter.FindProperty("_rerollCostText").objectReferenceValue = rerollButtonGO.GetComponentInChildren<TextMeshProUGUI>();
            serializedPresenter.FindProperty("_currentScrapText").objectReferenceValue = scrapTextGO.GetComponent<TextMeshProUGUI>();
            serializedPresenter.FindProperty("_shopItemPrefab").objectReferenceValue = itemPrefabGO.GetComponent<ShopItemUI>();
            serializedPresenter.ApplyModifiedProperties();

            Selection.activeGameObject = canvasGO;
            Debug.Log("Shop UI generated successfully! Please assign your WeaponConfigSOs to the ShopPresenter.");
        }

        private static GameObject CreateShopItemPrefab()
        {
            GameObject prefabGO = CreateUIElement("ShopItemPrefab_Generated", null);
            Image bg = prefabGO.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.15f, 1f);
            
            VerticalLayoutGroup layout = prefabGO.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(15, 15, 15, 15);
            layout.spacing = 10;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;

            GameObject lockIcon = CreateUIElement("LockIcon", prefabGO.transform);
            Image lockImg = lockIcon.AddComponent<Image>();
            lockImg.color = Color.red;
            LayoutElement lockLE = lockIcon.AddComponent<LayoutElement>();
            lockLE.preferredWidth = 30;
            lockLE.preferredHeight = 30;

            GameObject nameText = CreateTextElement("NameText", prefabGO.transform, "Item Name", 24);
            nameText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            GameObject iconGO = CreateUIElement("ItemIcon", prefabGO.transform);
            Image iconImg = iconGO.AddComponent<Image>();
            iconImg.color = Color.white;
            LayoutElement iconLE = iconGO.AddComponent<LayoutElement>();
            iconLE.preferredHeight = 100;
            iconLE.flexibleWidth = 1;

            GameObject statsText = CreateTextElement("StatsText", prefabGO.transform, "Stats...", 18);
            LayoutElement statsTextLE = statsText.AddComponent<LayoutElement>();
            statsTextLE.flexibleHeight = 1;

            GameObject buyButton = CreateButtonElement("BuyButton", prefabGO.transform, "Buy");
            LayoutElement buyLE = buyButton.AddComponent<LayoutElement>();
            buyLE.preferredHeight = 50;

            ShopItemUI shopItemUI = prefabGO.AddComponent<ShopItemUI>();
            
            SerializedObject serializedUI = new SerializedObject(shopItemUI);
            serializedUI.FindProperty("_buyButton").objectReferenceValue = buyButton.GetComponent<Button>();
            serializedUI.FindProperty("_nameText").objectReferenceValue = nameText.GetComponent<TextMeshProUGUI>();
            serializedUI.FindProperty("_statsText").objectReferenceValue = statsText.GetComponent<TextMeshProUGUI>();
            serializedUI.FindProperty("_lockIcon").objectReferenceValue = lockImg;
            serializedUI.FindProperty("_costText").objectReferenceValue = buyButton.GetComponentInChildren<TextMeshProUGUI>();
            serializedUI.ApplyModifiedProperties();

            // Save as Prefab
            string prefabPath = "Assets/Prefabs/ShopItemUI.prefab";
            System.IO.Directory.CreateDirectory("Assets/Prefabs");
            GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(prefabGO, prefabPath);
            DestroyImmediate(prefabGO);

            return savedPrefab;
        }

        // --- Helpers ---

        private static GameObject CreateUIElement(string name, Transform parent)
        {
            GameObject go = new GameObject(name);
            go.AddComponent<RectTransform>();
            if (parent != null) go.transform.SetParent(parent, false);
            return go;
        }

        private static GameObject CreateTextElement(string name, Transform parent, string text, int fontSize)
        {
            GameObject go = CreateUIElement(name, parent);
            TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = Color.white;
            return go;
        }

        private static GameObject CreateButtonElement(string name, Transform parent, string buttonText)
        {
            GameObject go = CreateUIElement(name, parent);
            Image img = go.AddComponent<Image>();
            img.color = new Color(0.3f, 0.3f, 0.3f);
            Button btn = go.AddComponent<Button>();
            
            GameObject textGO = CreateTextElement("Text", go.transform, buttonText, 24);
            textGO.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            textGO.GetComponent<TextMeshProUGUI>().color = Color.white;
            
            SetRectTransform(textGO, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one);
            
            return go;
        }

        private static void SetRectTransform(GameObject go, Vector2 anchoredPosition, Vector2 sizeDelta, Vector2 anchorMin, Vector2 anchorMax)
        {
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.sizeDelta = sizeDelta;
            rt.anchoredPosition = anchoredPosition;
        }
    }
}
#endif