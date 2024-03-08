using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSO
{
	enum KillType
	{
		NORMAL,
		STEALTH,
		CLOSEKILL
	}
	struct ScoreValue
	{
		KillType type;
		int value;
		public ScoreValue(KillType _type, int _value)
		{
			type = _type;
			value = _value;
		}
	}
	static ScoreValue normalKill = new ScoreValue(KillType.NORMAL, 100);
    static ScoreValue stealthKill = new ScoreValue(KillType.STEALTH, 150);
    static ScoreValue closeKill = new ScoreValue(KillType.CLOSEKILL, 200);
}
