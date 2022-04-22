using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface Tester
{
	public bool Authenticate();
}

public class ProxyTimeLord: TimeLord
{

    private StreamWriter _file;
    private string _time;
    private List<string> _logLineItems;
    private bool _activated;

    public ProxyTimeLord(int totalFrames, bool activate): base(totalFrames)
    {
        // Use _time if you want to store different log files each time.
        _time = DateTime.Now.ToString("dd-MM-yyyy-HH-mm");
        _file = new StreamWriter(Constants.LogFolder + "timeLog.txt");
        _file.WriteLine(totalFrames.ToString());

        _logLineItems = new List<string>();

        _activated = activate;
    }

    public override void Tick()
    {
        base.Tick();

        if (_activated)
        {
            _logLineItems.Add($"p-{_myID}-{_currentFrame}");

            string line = String.Join(",", _logLineItems.ToArray());
            _file.WriteLine(line);

            _logLineItems = new List<string>();
        }

        if (_currentFrame == _totalFrames - 1 && _activated)
        {
            _activated = false;
            WriteFinalStates(Constants.LogFolder + "stateLog.txt");
        }
    }

    public override void RecordState(PlayerState ps)
    {
        if (_activated)
        {
            int lastTailID = _realities.GetLastTailID(ps.PlayerID);
            List<int> frames = _realities.GetWriteFrames(ps.PlayerID);
            for (int i=0; i < frames.Count; i++)
            {
                int tailID = lastTailID + i;
                int frame = frames[i];
                _logLineItems.Add($"t-{tailID}-{frame}");
            }
        }

        base.RecordState(ps);
    }

    private void WriteFinalStates(string filename)
    {
        using StreamWriter file = new StreamWriter(filename);

        for (int i=0; i < _playerStates.Length; i++)
		{
			StringBuilder sb = new StringBuilder(100);

			sb.Append(i.ToString());

			if (_playerStates[i] != null)
			{
				foreach (var item in _playerStates[i])
				{
					string tail = item.Key.ToString();
					sb.Append($"-{tail}");
				}
			}

			file.WriteLine(sb.ToString());
		}
    }

	public void SnapshotStates(string filename)
	{
		using StreamWriter file = new StreamWriter(filename);

		for (int i=0; i < _playerStates.Length; i++)
		{
			StringBuilder sb = new StringBuilder(55);

			sb.Append(i.ToString("D4"));

			if (_playerStates[i] != null)
			{
				foreach (var item in _playerStates[i])
				{
					string tail = item.Key.ToString();
					sb.Append($" - {tail}");
				}
			}

			file.WriteLine(sb.ToString());
		}
	}

    public Dictionary<int, PlayerState>[] RevealPlayerStates(Tester tester)
	{
		if (tester.Authenticate()) return _playerStates;
		else throw new InvalidOperationException("Must be a Tester to call this method.");
	}

    public RealityManager RevealRealityManager(Tester tester)
	{
		if (tester.Authenticate()) return _realities;
		else throw new InvalidOperationException("Must be a Tester to call this method.");
	}
}
