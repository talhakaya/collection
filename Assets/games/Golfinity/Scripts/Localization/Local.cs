using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Games.Golfinity
{
	public static class Local {
	    public static OnLanguageChangeHandler OnLanguageChange;
	    public delegate void OnLanguageChangeHandler();

	    static Local() {
	        TextAsset textAsset = Resources.Load("localization") as TextAsset;
	        ParseLocalizationData(textAsset.text);
	    }

	    private static bool inited = false;

	    public static void SetLanguage(Lang lang) {
	        if (all != null && all.ContainsKey(lang.ToString())) {
	            m_current = all[lang.ToString()];

	            if (OnLanguageChange != null && inited)
	                OnLanguageChange();
	        }
	        else {
	            Debug.LogWarning(string.Format("Local cannot set language to: {0}", lang.ToString()));
	        }
	    }

	    public static string Get(string pKey) {
	        if (m_current != null && m_current.ContainsKey(pKey))
	            return m_current[pKey];
	        if (!inited)
	            return pKey;

	        if (all.ContainsKey("EN") && all["EN"].ContainsKey(pKey))
	            return all["EN"][pKey];

	        Debug.LogWarning(string.Format("Local cannot find key: {0}", pKey));

	        return pKey;
	    }

	    private static Dictionary<string, Dictionary<string, string>> all;
	    private static Dictionary<string, string> m_current;

	    private static void ParseLocalizationData(string fileText) {
	        string[,] csv = CSVReader.SplitCsvGrid(fileText);

	        all = new Dictionary<string, Dictionary<string, string>>();
	        for (int x = 0; x < csv.GetUpperBound(0); x++) {
	            if (string.IsNullOrEmpty(csv[x, 0]))
	                continue;
	            all.Add(csv[x, 0], new Dictionary<string, string>());
	        }

	        for (int x = 2; x < csv.GetUpperBound(0) - 1; x++) {
	            for (int y = 1; y < csv.GetUpperBound(1); y++) {
	                if (string.IsNullOrEmpty(csv[0, y]))
	                    continue;
	                
	                string key = csv[0, y];
	                string val = csv[x, y];

	                if (all[csv[x, 0]].ContainsKey(key)) {
	                    Debug.Log(key);
	                }
	                else {
	                    all[csv[x, 0]].Add(key, val);
	                }
	            }
	        }

	        inited = true;

	        if (OnLanguageChange != null)
	            OnLanguageChange();
	    }
	}
}
