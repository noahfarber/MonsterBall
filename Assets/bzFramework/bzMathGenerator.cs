using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using BZFramework.Math;
using Newtonsoft;
using System.Linq;
using Framework;

public class bzMathGenerator : MonoBehaviour
{
    public DazzleOutcome Outcome;
    public static BZStandardRNG RNG = new BZStandardRNG();
    public static BZMathWeightTable WeightTable = new BZMathWeightTable(RNG);
    public BZMathBucketManager BucketManager = new BZMathBucketManager(RNG, WeightTable);
    
    private void Awake()
    {
        LoadJsonData();
        BuildBuckets();
    }

    public DazzleOutcome RequestOutcome()
    {
        Outcome = BucketManager.PickOutcome() as DazzleOutcome;
        Debugger.Instance.Log(Outcome.ToString());
        return Outcome;
    }

    void LoadJsonData()
    {
        var weightText = Resources.Load<TextAsset>("dh_weights");
        var weightJson = JSON.Parse(weightText.ToString());

        foreach (var weight in weightJson)
        {
            int bonusCode = int.Parse(weight.Value["bonusCode"]);
            int pay = int.Parse(weight.Value["pay"]);
            int hits = int.Parse(weight.Value["hits"]);

            BZMathWeightBase bzWeight = new BZMathWeightBase() { Key = new TestDazzleKey(bonusCode, pay), Weight = hits};

            WeightTable.AddWeight(bzWeight);
        }

    }

    bool BonusFilter(IBZMathOutcome oc)
    {
        DazzleOutcome dazzleOutcome = oc as DazzleOutcome;
        BZMathKeyBase keyBase = dazzleOutcome.Key as BZMathKeyBase;

        return ((dazzleOutcome != null) && (keyBase != null) && (keyBase.BonusCode >= 2));
    }


    bool Bonus2xFilter(IBZMathOutcome oc)
    {
        DazzleOutcome dazzleOutcome = oc as DazzleOutcome;
        BZMathKeyBase keyBase = dazzleOutcome.Key as BZMathKeyBase;

        return ((dazzleOutcome != null) && (keyBase != null) && (keyBase.BonusCode == 2));
    }


    bool Bonus3xFilter(IBZMathOutcome oc)
    {
        DazzleOutcome dazzleOutcome = oc as DazzleOutcome;
        BZMathKeyBase keyBase = dazzleOutcome.Key as BZMathKeyBase;

        return ((dazzleOutcome != null) && (keyBase != null) && (keyBase.BonusCode == 3));
    }


    bool Bonus4xFilter(IBZMathOutcome oc)
    {
        DazzleOutcome dazzleOutcome = oc as DazzleOutcome;
        BZMathKeyBase keyBase = dazzleOutcome.Key as BZMathKeyBase;

        return ((dazzleOutcome != null) && (keyBase != null) && (keyBase.BonusCode == 4));
    }

    bool Bonus5xFilter(IBZMathOutcome oc)
    {
        DazzleOutcome dazzleOutcome = oc as DazzleOutcome;
        BZMathKeyBase keyBase = dazzleOutcome.Key as BZMathKeyBase;

        return ((dazzleOutcome != null) && (keyBase != null) && (keyBase.BonusCode == 5));
    }

    bool TopAwardFilter(IBZMathOutcome oc)
    {
        DazzleOutcome dazzleOutcome = oc as DazzleOutcome;
        BZMathKeyBase keyBase = dazzleOutcome.Key as BZMathKeyBase;

        return ((dazzleOutcome != null) && (keyBase != null) && (keyBase.BonusCode == 5) && (oc.TotalAward == 15000));
    }

    bool MultiBarFilter(IBZMathOutcome oc)
    {
        DazzleOutcome dazzleOutcome = oc as DazzleOutcome;
        BZMathKeyBase keyBase = dazzleOutcome.Key as BZMathKeyBase;

        return ((dazzleOutcome != null) 
            && (keyBase != null)
            && (dazzleOutcome.GetSpin(0).syms == "5,5,6" || dazzleOutcome.GetSpin(0).syms == "5,5,7" || dazzleOutcome.GetSpin(0).syms == "5,6,5" ||
            dazzleOutcome.GetSpin(0).syms == "5,7,5" || dazzleOutcome.GetSpin(0).syms == "7,5,5" || dazzleOutcome.GetSpin(0).syms == "6,5,5" ||
            dazzleOutcome.GetSpin(0).syms == "6,6,7" || dazzleOutcome.GetSpin(0).syms == "5,6,6" || dazzleOutcome.GetSpin(0).syms == "7,6,7"));
    }

    void BuildBuckets()
    {
        var jsonData = Resources.Load<TextAsset>("dh_buckets");
        Dictionary<int, Dictionary<int, List<JSONDazzleOutcome>>> buckets = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, List<JSONDazzleOutcome>>>>(jsonData.text);

        foreach (int bonusCode in buckets.Keys.ToList())
        {
            foreach (int award in buckets[bonusCode].Keys.ToList())
            {
                foreach (JSONDazzleOutcome outcome in buckets[bonusCode][award])
                {
                    BucketManager.AddOutcome(new DazzleOutcome(outcome));
                }
            }
        }

        BucketManager.SetPreFilter("bonus", BucketManager.FilterOC(BonusFilter));
        BucketManager.SetPreFilter("bonus2x", BucketManager.FilterOC(Bonus2xFilter));
        BucketManager.SetPreFilter("bonus3x", BucketManager.FilterOC(Bonus3xFilter));
        BucketManager.SetPreFilter("bonus4x", BucketManager.FilterOC(Bonus4xFilter));
        BucketManager.SetPreFilter("bonus5x", BucketManager.FilterOC(Bonus5xFilter));
        BucketManager.SetPreFilter("topAward", BucketManager.FilterOC(TopAwardFilter));
        BucketManager.SetPreFilter("multiBar", BucketManager.FilterOC(MultiBarFilter));
    }

    public float TestWeights()
    {
        TextAsset weightText = Resources.Load<TextAsset>("dh_weights");
        JSONNode weightJson = JSON.Parse(weightText.ToString());
        float bonusCount = 0;
        float weightCount = 0;
        float totalPay = 0;
        float rtp;

        foreach (var weight in weightJson)
        {
            int bonusCode = int.Parse(weight.Value["bonusCode"]);
            int pay = int.Parse(weight.Value["pay"]);
            int hits = int.Parse(weight.Value["hits"]);

            if(bonusCode > 0)
            {
                bonusCount += hits;
            }

            weightCount += hits;
            totalPay += pay * hits;
        }

        rtp = totalPay / (weightCount * 5);

        Debug.Log("Weights Test" 
            + System.Environment.NewLine + $"Weight Count: {weightCount:N0}" 
            + System.Environment.NewLine + $"Total Pay: {totalPay:N0}" 
            + System.Environment.NewLine + $"Bonus Rate: {(bonusCount / weightCount):P2}"
            + System.Environment.NewLine + $"RTP:  {rtp:P2}");

        return rtp;
    }

    public List<int> StringToIntList(string str)
    {
        List<int> rtn = new List<int>();

        if (!string.IsNullOrEmpty(str))
        {
            foreach (var s in str.Split(','))
            {
                int num;
                if (int.TryParse(s, out num))
                {
                    rtn.Add(num);
                }
                else
                {
                    Debug.LogError("Couldn't convert string : " + s);
                }
            }
        }

        return rtn;
    }
}
