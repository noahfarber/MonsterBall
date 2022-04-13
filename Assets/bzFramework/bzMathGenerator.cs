using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using BZFramework.Math;
using System.Linq;
using Framework;

public class bzMathGenerator : MonoBehaviour
{
    [SerializeField] bool LoadMathOnStart = true; 
    public DazzleOutcome Outcome;
    public static BZStandardRNG RNG = new BZStandardRNG();
    public static BZMathWeightTable WeightTable = new BZMathWeightTable(RNG);
    public BZMathBucketManager BucketManager = new BZMathBucketManager(RNG, WeightTable);
    
    private void Awake()
    {
        if(LoadMathOnStart)
        {
            LoadMath();
        }
    }

    public void LoadMath()
    {/*
        LoadJsonData();
        BuildBuckets();*/
        StartCoroutine(LoadCoroutine());
    }

    public DazzleOutcome RequestOutcome()
    {
        Outcome = BucketManager.PickOutcome() as DazzleOutcome;
        
        if(Outcome != null)
        {
            Debugger.Instance.Log(Outcome.ToString());
        }

        return Outcome;
    }

    public DazzleOutcome RequestFilteredOutcome(string filter)
    {
        Outcome = BucketManager.PickFilteredOutcome(filter) as DazzleOutcome;
        Debugger.Instance.Log(Outcome.ToString());
        return Outcome;
    }

    private IEnumerator LoadCoroutine()
    {
        Debugger.Instance.Log("PREPARING TO LOAD WEIGHTS");
        yield return null;
        var weightText = Resources.Load<TextAsset>("dh_weights");
        yield return null;
        Debugger.Instance.Log("WEIGHTS LOADED");
        yield return null;
        var weightJson = JSON.Parse(weightText.ToString());
        yield return null;
        Debugger.Instance.Log("WEIGHTS PARSED");
        yield return null;

        foreach (var weight in weightJson)
        {
            int bonusCode = int.Parse(weight.Value["bonusCode"]);
            int pay = int.Parse(weight.Value["pay"]);
            int hits = int.Parse(weight.Value["hits"]);

            BZMathWeightBase bzWeight = new BZMathWeightBase() { Key = new TestDazzleKey(bonusCode, pay), Weight = hits };

            WeightTable.AddWeight(bzWeight);
        }
        yield return null;

        Debugger.Instance.Log("WEIGHTS FINISHED");
        yield return null;






        var jsonData = Resources.Load<TextAsset>("dh_buckets");
        yield return null;
        Debugger.Instance.Log("BUCKETS LOADED");
        yield return null;
        Dictionary<int, Dictionary<int, List<JSONDazzleOutcome>>> buckets = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, List<JSONDazzleOutcome>>>>(jsonData.ToString());
        yield return null;
        Debugger.Instance.Log("BUCKETS DESERIALIZED.. Count: " + buckets.Count);
        yield return null;

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
        yield return null;

        BucketManager.SetPreFilter("bonus", BucketManager.FilterOC(BonusFilter));
        BucketManager.SetPreFilter("bonus2x", BucketManager.FilterOC(Bonus2xFilter));
        BucketManager.SetPreFilter("bonus3x", BucketManager.FilterOC(Bonus3xFilter));
        BucketManager.SetPreFilter("bonus4x", BucketManager.FilterOC(Bonus4xFilter));
        BucketManager.SetPreFilter("bonus5x", BucketManager.FilterOC(Bonus5xFilter));
        BucketManager.SetPreFilter("topAward", BucketManager.FilterOC(TopAwardFilter));
        BucketManager.SetPreFilter("multiBar", BucketManager.FilterOC(MultiBarFilter));
        yield return null;
        Debugger.Instance.Log("BUCKETS FINISHED");
        yield return null;
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
        Debugger.Instance.Log("BUCKETS LOADED");
        Dictionary<int, Dictionary<int, List<JSONDazzleOutcome>>> buckets = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, List<JSONDazzleOutcome>>>>(jsonData.ToString());
        Debugger.Instance.Log("BUCKETS DESERIALIZED.. Count: " + buckets.Count);

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
        Debugger.Instance.Log("BUCKETS FINISHED");

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

[System.Serializable]
public class PlayerInfo
{
    public string name;
    public int lives;
    public float health;

    public static PlayerInfo CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PlayerInfo>(jsonString);
    }

    // Given JSON input:
    // {"name":"Dr Charles","lives":3,"health":0.8}
    // this example will return a PlayerInfo object with
    // name == "Dr Charles", lives == 3, and health == 0.8f.
}
