using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public TMP_InputField monsterCountInput;
    public TMP_InputField heroCountInput;
    public TMP_InputField attackRangeInput;
    public TMP_InputField searchTimesInput;
    public TMP_InputField cellSizeInput;
    public TMP_Text cosText;
    public TMP_Text fpsText;
    public TMP_Text searchTypeText;

    private long lastCost;
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
        cellSizeInput.text = Grid.Instance.cellSize.ToString();
        searchTypeText.text = SearchCenter.Instance.searchType.ToString();
        cosText.text = "0us";
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

        if (lastCost != SearchCenter.Instance.costTime)
        {
            lastCost = SearchCenter.Instance.costTime;
            cosText.text = $"{lastCost:N0}";
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
        Grid.Instance.cellSize = n;
    }

    public void OnSearchTypeClick()
    {
        SearchCenter.Instance.NextSearchType();
        searchTypeText.text = SearchCenter.Instance.searchType.ToString();
        var show = SearchCenter.Instance.searchType == SearchType.Grid;
        cellSizeInput.transform.parent.gameObject.SetActive(show);
        Grid.Instance.gameObject.SetActive(show);
    }
}
