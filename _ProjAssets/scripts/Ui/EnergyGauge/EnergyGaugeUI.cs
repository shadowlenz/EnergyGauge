///written by: Eugene Chu
///https://twitter.com/LenZ_Chu
///free to use. please credit
//v.5.0
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class EnergyGaugeUI : MonoBehaviour
{
    public int current=100;
    public int max =100;
    [Header("Setup")]
    public bool showMask = true;
    public bool showMaskGraphic = true;
    public Sprite maskImg;
    public Color maskCol = new Color(0,0,0,0.3f);
    public Sprite barImg;
    public Vector2 size = new Vector2(100,100);
    [Header("image Type")]
    public bool preserveAspect = true;
    public enum BarType { Fill, Scale }
    [Space()]
    public BarType barType;

    public Image.FillMethod fillMethod = Image.FillMethod.Radial360;
    public Image.Origin360 _360Origin = Image.Origin360.Top;
    public Image.Origin90 _90Origin = Image.Origin90.BottomLeft;
    public Image.Origin180 _180rigin = Image.Origin180.Bottom;
    public Image.OriginHorizontal _HorizontalOrigin = Image.OriginHorizontal.Left;
    public Image.OriginVertical _VerticalOrigin = Image.OriginVertical.Top;
    public bool clockWise;
    [Header("replace to val: [cur] = current | [max] = max | [%] = %100 | [req] = required")]
    public string displayStyle;
    public Text displayText;
    public TextMeshProUGUI displayTextTMP;

    [Header("health")]
    public Gradient colorOverTime = new Gradient();
    //debug
    [SerializeField]
    public float Percentage { get { return (float)current / (float)max; } }
    [SerializeField]
    public float RequiredPercentage { get { return (float)required / (float)max; } }

    [Header("blink")]
    public bool CanBlink;
    public Color blinkColor = Color.black;
    [Range(0, 1)]
    public float percToBlink = 0.25f;
    public float blinkLength = 0.5f;
    float _blinkLength;
    bool blinkOnOff = true;

    [Header("bleed")]
    public bool canBleed = true;
    public Color bleedColor = Color.red;
    public float bleedPause = 1;
    //debug
    float _bleedPause;
    float oldPercentage = -1;


    [Header("required")]
    public bool canRequired = true;
    public bool keepRequiredMaxOfCurrent;
    public enum BehindAhead { Behind, Ahead}
    public BehindAhead behindAhead = BehindAhead.Behind;

    public int required = 10;
    public Color req_normalColor = Color.green;
    public Color req_UnsufficentColor = Color.red;

    [System.Serializable]
    public class BarSet
    {
        [HideInInspector]
        public Image mask;
        [HideInInspector]
        public Image img;
        [HideInInspector]
        public Image requiredMask;
        [HideInInspector]
        public Image requiredImg;
        [HideInInspector]
        public Image bleedImg;
        [System.NonSerialized]
        public float bleedPercCheckPoint;
        [System.NonSerialized]
        public float perPerc;
    }
    [Header("debug")]
    public List<BarSet> barSets = new List<BarSet>(new BarSet[1]);
    //legacy
    public Image mask;
    public Image img;
    public Image requiredImg;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < barSets.Count; i++)
        {
            CreateMainGuage(i);
            CreateRequiredBar(i);
            CreateBleedBar(i);
        }

    }
    // Update is called once per frame
    void Update()
    {
        ConsistentPosSize();
        MainGauge();
        BleedGauge();
        RequiredGauge();
        BlinkLogic();
        DisplayLogic();
    }

    public void MainGauge()
    {
        //current limit
        if (current > max) current = max;
        else if (current < 0) current = 0;
        //color
        Color _col = colorOverTime.Evaluate(Percentage);
        if (!blinkOnOff) _col = blinkColor;

        for (int i = 0; i < barSets.Count; i++)
        {
            barSets[i].perPerc = Mathf.Clamp01 ((Percentage * barSets.Count) - i);
            if (barSets[i].img == null) return;

            barSets[i].img.color = _col;

            if (barType == BarType.Fill) barSets[i].img.fillAmount = barSets[i].perPerc;
            else if (barType == BarType.Scale) barSets[i].img.rectTransform.sizeDelta = size * barSets[i].perPerc;
        }
    }

    void BleedGauge()
    {
        if (!canBleed)
        {
            for (int i = 0; i < barSets.Count; i++)
            {
                if (barSets[i].bleedImg != null) barSets[i].bleedImg.enabled = false;
            }
            return;
        }

        bool bleedOver = false; //check if old percentage is decreasing than current perc
        if (oldPercentage != Percentage)
        {
            if (Percentage > 0)
            {
                if (oldPercentage > Percentage) //decreasing
                {
                    bleedOver = false;
                    _bleedPause = bleedPause;
                }
                else if (oldPercentage < Percentage) //increased value. snap to percentage
                {
                    bleedOver = true;
                    _bleedPause = 0;
                }
            }
            else
            {
                bleedOver = false;
                _bleedPause = 0;
            }
            oldPercentage = Percentage; //trigger
        }
        //bleed check point
        if (_bleedPause > 0 && !bleedOver) //RemainderRequired() required has num remained, inert this bleed logic
        {
            //pause time to see the bleed
            _bleedPause = Mathf.Clamp01( _bleedPause- Time.unscaledDeltaTime);
        }

        for (int i = 0; i < barSets.Count; i++)
        {
            if (barSets[i].bleedImg == null) return;
            //graphics
            barSets[i].bleedImg.enabled = true;
            barSets[i].bleedImg.sprite = barImg;
            barSets[i].bleedImg.color = bleedColor;
            //=================
            if (i< barSets.Count-1 && barSets[i + 1].bleedPercCheckPoint > 0) //the other bars are still at full
            {
                barSets[i].bleedPercCheckPoint = 1;
            }
            else
            {
                float _speed = 3;
                if (Percentage == 0 ) _speed = 10; //death

                if (barSets[i].bleedPercCheckPoint > barSets[i].perPerc)
                {
                    if (_bleedPause <= 0)
                    {
                        barSets[i].bleedPercCheckPoint -= Time.unscaledDeltaTime * _speed;
                    }
                }
                else
                {
                    barSets[i].bleedPercCheckPoint = barSets[i].perPerc;
                }

            }
            //========================
            if (barType == BarType.Fill)  barSets[i].bleedImg.fillAmount = barSets[i].bleedPercCheckPoint;
            else if (barType == BarType.Scale) barSets[i].bleedImg.rectTransform.sizeDelta = size * barSets[i].bleedPercCheckPoint;
        }
       
    }


    public void RequiredGauge()
    {
        if (!canRequired) return;
        //percent
        float perc = Percentage;
        if (behindAhead == BehindAhead.Behind)
        {
            if (keepRequiredMaxOfCurrent)
            {
                perc = Mathf.Max((float)required / (float)max, Percentage);
                //BarType.Scale offset do it again
                if (barType == BarType.Scale) perc = Mathf.Max((float)(required *2) / (float)max, Percentage);
            }
        }
        else if (behindAhead == BehindAhead.Ahead)
        {
            perc += RequiredPercentage;
            //BarType.Scale offset do it again
            if (barType == BarType.Scale) perc += RequiredPercentage;
        }
            //


        for (int i = 0; i < barSets.Count; i++)
        {
            //require mask
            if (barSets[i].requiredMask != null) 
            {
                ReverseImg(barSets[i].requiredMask);

                if (barType == BarType.Fill)
                {
                    float _invertFillAmount = (1 - // invert the percentage
                            ((perc * barSets.Count) - i) //multiple count per
                            ) + (RequiredPercentage * barSets.Count); //multiple count required per

                    barSets[i].requiredMask.fillAmount = _invertFillAmount;
                }
                else if (barType == BarType.Scale)
                {
                    barSets[i].requiredMask.rectTransform.sizeDelta = size;
                }
            }
            //main require guage
            if (barSets[i].requiredImg != null)
            {
                if (RemainderRequired() == 0) barSets[i].requiredImg.color = req_normalColor;
                else barSets[i].requiredImg.color = req_UnsufficentColor;

                if (barType == BarType.Fill)
                {
                    float _fillAmount = ((perc * barSets.Count) - i);
                    barSets[i].requiredImg.fillAmount = _fillAmount;//multiple count per
                }
                else if (barType == BarType.Scale)
                {
                    float _fillAmount = ((perc * barSets.Count) - i) - (RequiredPercentage * barSets.Count);

                    barSets[i].requiredImg.rectTransform.sizeDelta = size * Mathf.Clamp01(_fillAmount);
                }

            }

        }
        if (required < 0) required = 0;

    }

    void BlinkLogic()
    {
        if (!CanBlink) return;

        if (Percentage <= percToBlink)
        {
            if (_blinkLength <= 0) {
                blinkOnOff = !blinkOnOff;
                _blinkLength = blinkLength;
            }
            else
            {
                _blinkLength -= Time.unscaledDeltaTime;
            }
        }
        else
        {
            blinkOnOff = true;
        }
    }

    public void DisplayLogic()
    {   
        if (displayText == null && displayTextTMP == null) return;
        string parse = displayStyle;

        parse= parse.Replace("[cur]", current.ToString());
        parse= parse.Replace("[max]", max.ToString());
        parse = parse.Replace("[req]", required.ToString());
        parse = parse.Replace("[%]", ( Mathf.RoundToInt( Percentage * 100) ).ToString());

        if (displayText != null )  displayText.text = parse;
        if (displayTextTMP != null) displayTextTMP.text = parse;
    }
    public void ConsistentPosSize()
    {
        for (int i = 0; i < barSets.Count; i++)
        {
            if (barSets[i].mask != null)
            {
                SetupImg(barSets[i].mask, true);
                barSets[i].mask.color = maskCol;
            }
            SetupImg(barSets[i].img);
            SetupImg(barSets[i].requiredMask);
            SetupImg(barSets[i].requiredImg);
            SetupImg(barSets[i].bleedImg);
        }
    }
    ///TOOLS/////////////////////////////////////////////////
    float RemainderRequired()
    {
        float remainder = 0;
        if (canRequired)
        {
            if (behindAhead == BehindAhead.Behind && RequiredPercentage > Percentage) remainder = Percentage - RequiredPercentage;
            if (behindAhead == BehindAhead.Ahead && (Percentage + RequiredPercentage) > 1) remainder = (Percentage + RequiredPercentage) -1;
        }
      // Debug.Log(remainder);
        return remainder;
    }
    void SetupImg(Image img, bool ignoreImgTypeAndMethod = false)
    {
        if (img == null) return;

        img.rectTransform.sizeDelta = size;
        img.preserveAspect = preserveAspect;
        if (!ignoreImgTypeAndMethod)
        {
            img.rectTransform.anchoredPosition = Vector2.zero;
            if (barType == BarType.Fill)
            {
                img.type = Image.Type.Filled;
                img.fillMethod = fillMethod;
                if (fillMethod == Image.FillMethod.Radial360) img.fillOrigin = (int)_360Origin;
                else if (fillMethod == Image.FillMethod.Radial180) img.fillOrigin = (int)_180rigin;
                else if (fillMethod == Image.FillMethod.Radial90) img.fillOrigin = (int)_90Origin;
                else if (fillMethod == Image.FillMethod.Horizontal) img.fillOrigin = (int)_HorizontalOrigin;
                else if (fillMethod == Image.FillMethod.Vertical) img.fillOrigin = (int)_VerticalOrigin;
                img.fillClockwise = clockWise;
            }
            else if (barType == BarType.Scale)
            {
                img.type = Image.Type.Simple;
            }
        }
    }
    void ReverseImg(Image _img)
    {
        if (barSets == null) return;

        if (fillMethod == Image.FillMethod.Radial360 || fillMethod == Image.FillMethod.Radial180 || fillMethod == Image.FillMethod.Radial90)
        {
            _img.fillClockwise = !clockWise;
            //_img.rectTransform.anchoredPosition = new Vector2(0,0);
        }
        else if (fillMethod == Image.FillMethod.Horizontal)
        {
            if (_HorizontalOrigin == (int)Image.OriginHorizontal.Left)
            {
                _img.fillOrigin = (int)Image.OriginHorizontal.Right;
            }
            else
            {
                _img.fillOrigin = (int)Image.OriginHorizontal.Left;
            }

        }
        else if (fillMethod == Image.FillMethod.Vertical)
        {
            if (_HorizontalOrigin == (int)Image.OriginVertical.Bottom)
            {
                _img.fillOrigin = (int)Image.OriginVertical.Top;
            }
            else
            {
                _img.fillOrigin = (int)Image.OriginVertical.Bottom;
            }

        }
    }

    /////public////
   public void SetValueCurrent(int _current)
    {
      current =  _current;
    }
    public void SetValueMax(int _max)
    {
        max = _max;
    }
    //runtime replace
    public void AddNotch()
    {
        barSets.Add(new BarSet());

        int lastId = barSets.Count - 1;
        CreateMainGuage(lastId);
        CreateBleedBar(lastId);
        CreateRequiredBar(lastId);
    }
    public void RemoveNotch()
    {
        int lastId = barSets.Count - 1;

        if (barSets[lastId].mask != null) Destroy(barSets[lastId].mask.gameObject);
        if (barSets[lastId].img != null) Destroy(barSets[lastId].img.gameObject);
        if (barSets[lastId].requiredMask != null) Destroy(barSets[lastId].requiredMask.gameObject);
        if (barSets[lastId].requiredImg != null) Destroy(barSets[lastId].requiredImg.gameObject);
        if (barSets[lastId].bleedImg != null) Destroy(barSets[lastId].bleedImg.gameObject);

        barSets.RemoveAt(lastId);
    }
    public void CreateMainGuage(int ID)
    {
        if (barSets[ID].mask == null)
        {
            //mask
            GameObject _mask = new GameObject("Mask_" + ID);
            _mask.transform.SetParent(transform);
            Image _maskImg = _mask.AddComponent<Image>();
            Mask _mc = _mask.AddComponent<Mask>();

            _maskImg.enabled = showMask;
            _maskImg.sprite = maskImg;
            _mc.showMaskGraphic = showMaskGraphic;


            barSets[ID].mask = _maskImg;
        }
        if (barSets[ID].img == null)
        {
            GameObject _gauge = new GameObject("mainGauge_" + ID);
            _gauge.transform.SetParent(barSets[ID].mask.transform);
            Image _img = _gauge.AddComponent<Image>();

            barSets[ID].img = _img;
            barSets[ID].img.sprite = barImg;


        }

    }
    public void CreateRequiredBar(int ID)
    {
        if (canRequired)
        {
            //mask
            if (barSets[ID].requiredMask == null)
            {
                GameObject _mask = new GameObject("RequiredMask_" + ID);
                _mask.transform.SetParent(barSets[ID].mask.transform);
                Image _maskImg = _mask.AddComponent<Image>();
                Mask _mc = _mask.AddComponent<Mask>();
                _mc.showMaskGraphic = false;

                barSets[ID].requiredMask = _maskImg;

                _maskImg.sprite = barImg;
            }

            //guage
            if (barSets[ID].requiredImg == null)
            {
                Image requiredImg = Instantiate(barSets[ID].img.gameObject).GetComponent<Image>();
                requiredImg.name = "Required_" + ID;
                requiredImg.transform.SetParent(barSets[ID].requiredMask.transform);

                barSets[ID].requiredImg = requiredImg;

                requiredImg.sprite = barImg;
            }

        }
    }

     public void CreateBleedBar(int ID)
    {
        if (canBleed && barSets[ID].img != null && barSets[ID].bleedImg == null)
        {
            barSets[ID].bleedImg = Instantiate(barSets[ID].img);
            barSets[ID].bleedImg.transform.SetParent(barSets[ID].img.transform.parent);
            barSets[ID].bleedImg.name = "Bleed_" + ID;
            barSets[ID].bleedImg.sprite = barImg;

            barSets[ID].bleedImg.transform.SetSiblingIndex(0);
        }

    }
}
