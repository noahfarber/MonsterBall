using BZFramework.Math;
using System.Collections.Generic;
using System.Linq;

public class JSONDazzleWeight
{
    public int pay { get; set; }
    public int hits { get; set; }
    public int bonusCode { get; set; }
}

public class JSONDazzleOutcome
{
    public string stops { get; set; }
    public int baseAward { get; set; }
    public int totalAward { get; set; }
    public JSONDazzleOutcome[] spins { get; set; }
}



public class TestDazzleKey : BZMathKeyBase
{
    public TestDazzleKey(int aCode, int aPay)
    {
        BonusCode = aCode;
        Award = aPay;
    }
}

public class DazzleSpinData
{
    public string stops { get; set; }
    public int baseAward { get; set; }
    public int totalAward { get; set; }

    public DazzleSpinData(JSONDazzleOutcome js = null)
    {
        if (js == null)
        {
            stops = "";
            baseAward = 0;
            totalAward = 0;
        }
        else
        {
            stops = js.stops;
            baseAward = js.baseAward;
            totalAward = js.totalAward;
        }
    }
}

public class DazzleOutcome : BZMathOutcomeBase
{

    public DazzleSpinData GetSpin(int idx)
    {
        DazzleSpinData rtn = null;
        if ((idx >= 0) && (idx < _spins.Count))
        {
            rtn = _spins[idx];
        }
        return rtn;
    }

    public List<DazzleSpinData> GetSpins()
    {
        return _spins.ToList();
    }

    public int SpinCount
    {
        get
        {
            return _spins.Count;
        }
    }

    public DazzleOutcome(JSONDazzleOutcome js = null)
    {
        if (js == null)  //  Init new outcome
        {
            _spins.Add(new DazzleSpinData());
        }
        else
        {
            _spins.Add(new DazzleSpinData(js));
            if (js.spins != null)
            {
                foreach (JSONDazzleOutcome extraSpin in js.spins)
                {
                    _spins.Add(new DazzleSpinData(extraSpin));
                }
            }
        }

        this.TotalAward = _spins[0].totalAward;
        this.Key = new TestDazzleKey((_spins.Count < 2) ? 0 : _spins.Count, this.TotalAward);
    }

    public override string ToString()
    {
        string rtn = "Outcome: Award: " + TotalAward + "    Spin Win: " + GetSpin(0).baseAward + "     Stops: " + GetSpin(0).stops + "    Total Spins: " + GetSpins().Count;
        return rtn;

    }

    private List<DazzleSpinData> _spins = new List<DazzleSpinData>();
}
