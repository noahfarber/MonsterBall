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
    [SerializeField] bool LoadReelsOnStart = true;
    public static BZStandardRNG RNG = new BZStandardRNG();
    public static BZMathWeightTable WeightTable = new BZMathWeightTable(RNG);
    public BZMathBucketManager BucketManager = new BZMathBucketManager(RNG, WeightTable);



    private void Start()
    {
        if (LoadMathOnStart)
        {
            LoadMath();
        }

        if (LoadMathOnStart)
        {
        }

    }

    public void LoadMath()
    {
        StartCoroutine(LoadCoroutine());
    }


    private IEnumerator LoadCoroutine()
    {
        Debugger.Instance.Log("PREPARING TO LOAD WEIGHTS");
        yield return null;
        var weightText = Resources.Load<TextAsset>("smallWeights");
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



        var jsonData = Resources.Load<TextAsset>("smallBuckets");
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
        Debugger.Instance.Log("BUCKETS FINISHED");
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

        Debugger.Instance.Log("BUCKETS FINISHED");

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