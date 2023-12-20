using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public TMP_InputField monsterCountInput;
    public TMP_InputField heroCountInput;
    public TMP_InputField attackRangeInput;
    public TMP_InputField searchTimesInput;
    public TMP_InputField cellSizeInput;
    public TMP_Text searchCostText;
    public TMP_Text searchCostAvgText;
    public TMP_Text buildCostText;
    public TMP_Text fpsText;
    public TMP_Text searchTypeText;

    private long lastSearchCost;
    private Queue<long> searchCost10 = new Queue<long>();
    private long lastBuildCost;
    private float fpsTimer;
    private int fpsCounter;
    private int fps;

    // Start is called before the first frame update
    void Start()
    {
        monsterCountInput.text = MonsterManager.Instance.count.ToString();
        heroCountInput.text = HeroManager.Instance.count.ToString();
        attackRangeInput.text = HeroManager.Instance.attackRange.ToString("f2");
        searchTimesInput.text = SearchCenter.Instance.searchTimes.ToString();
        cellSizeInput.text = GridX.Instance.cellSize.ToString();
        searchTypeText.text = SearchCenter.Instance.searchType.ToString();
        searchCostText.text = "0us";
        fpsText.text = "0";
    }

    // Update is called once per frame
    void Update()
    {
        fpsTimer += Time.unscaledDeltaTime;
        fpsCounter++;
        if (fpsTimer > 1)
        {
            fps = (int) (fpsCounter / fpsTimer);
            fpsTimer -= 1;
            fpsCounter = 0;
            fpsText.text = fps.ToString();
        }

        if (SearchCenter.Instance.searchCostTimeChanged)
        {
            SearchCenter.Instance.searchCostTimeChanged = false;
            if (lastSearchCost != SearchCenter.Instance.searchCostTime)
            {
                lastSearchCost = SearchCenter.Instance.searchCostTime;
                searchCostText.text = $"{lastSearchCost:N0}";
            }

            if (searchCost10.Count >= 10)
            {
                searchCost10.Dequeue();
            }
            searchCost10.Enqueue(lastSearchCost);
            long total = 0;
            foreach (long l in searchCost10)
            {
                total += l;
            }

            var n = searchCost10.Count > 0 ? total / searchCost10.Count : 0;
            searchCostAvgText.text = $"{n:N0}";
        }

        if (lastBuildCost != SearchCenter.Instance.buildCostTime)
        {
            lastBuildCost = SearchCenter.Instance.buildCostTime;
            buildCostText.text = $"{lastBuildCost:N0}";
        }
    }

    public void OnMonsterCountInput()
    {
        var n = 0; 
        int.TryParse(monsterCountInput.text, out n);
        MonsterManager.Instance.count = n;
    }

    public void OnHeroCountInput()
    {
        var n = 0;
        int.TryParse(heroCountInput.text, out n);
        HeroManager.Instance.count = n;
    }

    public void OnAttackRangeInput()
    {
        var n = 0f;
        float.TryParse(attackRangeInput.text, out n);
        HeroManager.Instance.attackRange = n;
    }

    public void OnSearchTimesInput()
    {
        var n = 0;
        int.TryParse(searchTimesInput.text, out n);
        SearchCenter.Instance.searchTimes = n;
    }

    public void OnCellSizeInput()
    {
        var n = 0;
        int.TryParse(cellSizeInput.text, out n);
        GridX.Instance.cellSize = n;
    }

    public void OnSearchTypeClick()
    {
        SearchCenter.Instance.NextSearchType();
        searchTypeText.text = SearchCenter.Instance.searchType.ToString();
        var show = SearchCenter.Instance.searchType == SearchType.Grid;
        cellSizeInput.transform.parent.gameObject.SetActive(show);
        GridX.Instance.gameObject.SetActive(show);
    }
}
