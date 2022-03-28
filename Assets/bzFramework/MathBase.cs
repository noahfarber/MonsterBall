using System;
using System.Collections.Generic;
using System.Threading;

namespace BZFramework.Math
{
    public class BZStandardRNG : IBZMathRandom
    {
        /*
         * Default thread-safe use of System.Random
         * Ensures that every thread gets a unique random object to avoid thread conflicts
         */

        int IBZMathRandom.Rand(int aMinVal, int aMaxVal)
        {
            if (aMaxVal < aMinVal)
            {
                int t = aMinVal;
                aMinVal = aMaxVal;
                aMaxVal = t;
            }
            return _rng.Next(aMinVal, aMaxVal + 1);
        }

        void IBZMathRandom.Reseed(int aSeed)
        {
            int tID = Thread.CurrentThread.ManagedThreadId;
            lock (_lockObj)
            {
                _rngs[tID] = new System.Random(aSeed);  //  Only way to set the seed of a System.Random is via constructor, so whether or not we already have one we must create a new one for this thread!
            }
        }

        private Dictionary<int, System.Random> _rngs = new Dictionary<int, Random>();
        private static readonly object _lockObj = new object();

        private System.Random _rng
        {
            get
            {
                int tID = Thread.CurrentThread.ManagedThreadId;
                lock (_lockObj)
                {
                    if (!_rngs.ContainsKey(tID))
                    {
                        _rngs[tID] = new System.Random(Environment.TickCount);
                    }
                }
                return _rngs[tID] ?? new System.Random(Environment.TickCount);
            }
        }
    }

    [Serializable]  //  Allow these classes to be saves/restored
    public class BZMathKeyBase : IBZMathOutcomeKey
    {
        public int BonusCode { get; set; }

        public int Award {  get; set; }

        public string Value
        {
            get
            {
                return $"{BonusCode}|{Award}";
            }
        }
    }

    [Serializable]
    public class BZMathWeightBase : IBZMathWeight
    {
        public IBZMathOutcomeKey Key { get; set; } = null;

        public int Weight { get; set; } = 0;
    }

    [Serializable]
    public class BZMathOutcomeBase : IBZMathOutcome
    {
        public IBZMathOutcomeKey Key { get; set; }

        public int TotalAward { get; set; } 
    }

    [Serializable]
    public class BZMathWeightTable : IBZMathWeightTable
    {
        public int TotalWeight
        {
            get
            {
                int rtn = 0;
                lock (_lockObj)
                {
                    foreach (IBZMathWeight w in _weights)
                    {
                        if (w.Weight > 0)
                        {
                            rtn += w.Weight;
                        }
                    }
                }
                return rtn;
            }
        }

        public int TotalCount
        {
            get
            {
                int rtn = 0;
                lock (_lockObj)
                {
                    rtn = _weights.Count;
                }
                return rtn;
            }
        }

        public IBZMathWeight SelectRandom()
        {
            IBZMathWeight rtn = null;
            int totWeight = TotalWeight;
            lock (_lockObj)
            {
                if (totWeight > 0)
                {
                    int aPick = _rng.Next(totWeight);
                    int tot = 0;
                    foreach (IBZMathWeight w in _weights)
                    {
                        if (w.Weight > 0)
                        {
                            tot += w.Weight;
                            if (tot > aPick)
                            {
                                rtn = w;
                                break;
                            }
                        }
                    }
                }
            }
            return rtn;
        }

        public void Clear()
        {
            lock (_lockObj)
            {
                _weights.Clear();
            }
        }

        public void AddWeight(IBZMathWeight w)
        {
            IBZMathWeight dup = GetWeightByKey(w.Key.Value);  //  Check for an entry that's already in the list...
            if (dup == null)  //  No such entry, proceed!
            {
                lock (_lockObj)
                {
                    if (!_weights.Contains(w))
                    {
                        _weights.Add(w);
                    }
                }
            }
        }
        public void RemoveWeight(IBZMathWeight w)
        {
            lock (_lockObj)
            {
                if (_weights.Contains(w))
                {
                    _weights.Remove(w);
                }
            }
        }

        public void RemoveWeightByKey(string aKey)
        {
            IBZMathWeight w = GetWeightByKey(aKey);
            if (w != null)
            {
                lock (_lockObj)
                {
                    RemoveWeight(w);
                }
            }
        }

        public IBZMathWeight GetWeightByKey(string aKey)
        {
            IBZMathWeight rtn = null;
            lock (_lockObj)
            {
                foreach (IBZMathWeight w in _weights)
                {
                    if (w.Key.Value.Equals(aKey))
                    {
                        rtn = w;
                        break;
                    }
                }
            }
            return rtn;
        }

        public IBZMathWeight GetWeightByIndex(int anIndex)
        {
            IBZMathWeight rtn = null;
            lock (_lockObj)
            {
                if ((anIndex >= 0) && (anIndex < _weights.Count))
                {
                    rtn = _weights[anIndex];
                }
            }
            return rtn;
        }

        public string SelectKey()
        {
            string rtn = null;
            IBZMathWeight w = SelectRandom();
            if (w != null)
            {
                rtn = w.Key.Value;
            }
            return rtn;
        }


        public BZMathWeightTable(IBZMathRandom anRNG = null)
        {
            _rng = anRNG ?? new BZStandardRNG();
        }

        private List<IBZMathWeight> _weights = new List<IBZMathWeight>();
        private IBZMathRandom _rng = null;
        private static readonly object _lockObj = new object();
    }

    [Serializable]
    public class BZMathBucketManager
    {
        public BZMathBucketManager(IBZMathRandom rng = null, IBZMathWeightTable wts = null)
        {
            if (rng == null)
            {
                _rng = new BZStandardRNG();
            }
            else
            {
                _rng = rng;
            }
            if (wts != null)
            {
                _wts = wts;
            }
        }

        public int BucketCount()
        {
            int rtn = 0;
            lock (_lockObj)
            {
                rtn = _buckets.Count;
            }
            return rtn;
        }

        public int OutcomeCount(string aKey)
        {
            int rtn = 0;
            lock(_lockObj)
            {
                if ((aKey == null) || (aKey.Equals("")))  //  Empty key so return all outcomes in all keys
                {
                    foreach (KeyValuePair<string, List<IBZMathOutcome>> kvp in _buckets)
                    {
                        rtn += kvp.Value.Count;
                    }
                }
                else  //  Just return outcomes for specified key
                {
                    if (_buckets.ContainsKey(aKey))
                    {
                        rtn = _buckets[aKey].Count;
                    }
                }
            }
            return rtn;
        }

        public IBZMathOutcome PickOutcome(string aKey = null)
        {
            //  Randomly pick one of the outcomes in the specified bucket
            IBZMathOutcome rtn = null;
            lock(_lockObj)
            {
                if ((aKey == null) && (_wts != null))
                {
                    IBZMathWeight w = _wts.SelectRandom();
                    if (w != null)
                    {
                        aKey = w.Key.Value;
                    }
                }
                List<IBZMathOutcome> pickOptions = new List<IBZMathOutcome>();
                if ((aKey == null) || (aKey.Equals("")))  //  Empty key, pick from all outcomes
                {
                    foreach (KeyValuePair<string, List<IBZMathOutcome>> kvp in _buckets)
                    {
                        pickOptions.AddRange(kvp.Value);
                    }
                }
                else  // Just pick from specified bucket
                {
                    if (_buckets.ContainsKey(aKey))
                    {
                        pickOptions = _buckets[aKey];
                    }
                }
                if ((pickOptions != null) && (pickOptions.Count > 0))  //  We have something to choose from...
                {
                    int pickIDX = _rng.Next(pickOptions.Count);
                    rtn = pickOptions[pickIDX];
                }
            }
            return rtn;
        }

        public IBZMathOutcome PickFilteredOutcome(string aFilterName)
        {
            IBZMathOutcome rtn = null;
            if (_rng != null)
            {
                lock (_lockObj)
                {
                    IList<IBZMathOutcome> pickOptions = _prefilters.ContainsKey(aFilterName) ? _prefilters[aFilterName] : null;
                    if ((pickOptions != null) && (pickOptions.Count > 0))
                    {
                        int pickIDX = _rng.Next(pickOptions.Count);
                        rtn = pickOptions[pickIDX];
                    }
                }
            }
            return rtn;
        }

        public IList<IBZMathOutcome> FilterKey(Predicate<IBZMathOutcomeKey> aPred)
        {
            return Filter(aPred, null);
        }

        public IList<IBZMathOutcome> FilterOC(Predicate<IBZMathOutcome> aPred)
        {
            return Filter(null, aPred);
        }

        public IList<IBZMathOutcome> Filter(Predicate<IBZMathOutcomeKey> aKeyPred, Predicate<IBZMathOutcome> anOCPred)
        {
            List<IBZMathOutcome> rtn = new List<IBZMathOutcome>();
            lock(_lockObj)
            {
                foreach (KeyValuePair<string, List<IBZMathOutcome>> kvp in _buckets)
                {
                    foreach (IBZMathOutcome oc in kvp.Value)
                    {
                        bool keyOK = ((aKeyPred == null) || (aKeyPred(oc.Key)));
                        bool ocOK = ((anOCPred == null) || (anOCPred(oc)));
                        if (keyOK && ocOK)
                        {
                            rtn.Add(oc);
                        }
                    }
                }
            }
            return rtn;
        }

        public void SetPreFilter(string aName, IList<IBZMathOutcome> filterList)
        {
            lock (_lockObj)
            {
                _prefilters[aName] = filterList;
            }
        }


        public void AddOutcome(IBZMathOutcome anOC)
        {
            lock (_lockObj)
            {
                if (!_buckets.ContainsKey(anOC.Key.Value))
                {
                    _buckets[anOC.Key.Value] = new List<IBZMathOutcome>();
                }
                _buckets[anOC.Key.Value].Add(anOC);
            }
        }

        public void RemoveOutcome(IBZMathOutcome anOC)
        {
            lock(_lockObj)
            {
                if (_buckets.ContainsKey(anOC.Key.Value))
                {
                    if (_buckets[anOC.Key.Value].Contains(anOC))
                    {
                        _buckets[anOC.Key.Value].Remove(anOC);
                    }
                }
            }
        }


        private Dictionary<string, List<IBZMathOutcome>> _buckets = new Dictionary<string, List<IBZMathOutcome>>();
        private Dictionary<string, IList<IBZMathOutcome>> _prefilters = new Dictionary<string, IList<IBZMathOutcome>>();
        private IBZMathRandom _rng = null;
        private IBZMathWeightTable _wts = null;
        private readonly static object _lockObj = new object();

    }
}
