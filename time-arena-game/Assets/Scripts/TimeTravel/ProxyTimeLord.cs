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

    private StreamWriter _logFile;
    private StreamWriter _diagnosticsFile;
    private List<string> _logLineItems;
    private bool _logging;
    private bool _diagnostics;
    private Dictionary<string, int> _diagnosticData;

    public ProxyTimeLord(int totalFrames, bool logging=false, bool diagnostics=false): base(totalFrames)
    {
        _logging = logging;
        _diagnostics = diagnostics;

        if (_logging)
        {
            _logFile = new StreamWriter(Constants.LogFolder + "timeLog.txt");
            _logFile.WriteLine(totalFrames.ToString());

            _logLineItems = new List<string>();
        }

        if (_diagnostics)
        {
            _diagnosticsFile = new StreamWriter(Constants.LogFolder + "diagnostics.txt");
            _diagnosticsFile.WriteLine($"Total frames: {totalFrames}");
            _diagnosticsFile.WriteLine($"FPS: {Constants.FrameRate}");
            _diagnosticsFile.WriteLine($"Frames between each synchronisation: {Constants.SyncFrames}");
            _diagnosticsFile.WriteLine("");

            _diagnosticData = new Dictionary<string, int>(){
                {"totalDifference", 0},
                {"totalBehind", 0},
                {"totalAhead", 0},
                {"numBehind", 0},
                {"numAhead", 0},
                {"synchronised", 0},
                {"syncs", 0}
            };
        }
    }

    public override void Tick()
    {
        base.Tick();

        if (_logging)
        {
            if (_myID != 0) _logLineItems.Add($"p-{_myID}-{_realities.GetPerceivedFrame(_myID)}");

            string line = String.Join(",", _logLineItems.ToArray());
            _logFile.WriteLine(line);

            _logLineItems = new List<string>();

            if (_currentFrame == _totalFrames - 1)
            {
                _logging = false;
                _logFile.Close();
                WriteFinalStates(Constants.LogFolder + "stateLog.txt");
            }
        }

        if (_diagnostics && _currentFrame == _totalFrames - 1)
        {
            _diagnostics = false;
            _diagnosticsFile.WriteLine("Number of synchronisations: {0}", _diagnosticData["syncs"]);
            int average = _diagnosticData["totalDifference"] / _diagnosticData["syncs"];
            _diagnosticsFile.WriteLine("Average difference between current frames: {0}", average);
            _diagnosticsFile.WriteLine("Number of times ahead: {0}", _diagnosticData["numAhead"]);
            _diagnosticsFile.WriteLine("Number of times behind: {0}", _diagnosticData["numBehind"]);
            _diagnosticsFile.WriteLine("Number of times in sync: {0}", _diagnosticData["synchronised"]);
            int averageBehind = _diagnosticData["totalBehind"] / _diagnosticData["numBehind"];
            _diagnosticsFile.WriteLine("Average frames behind: {0}", averageBehind);
            int averageAhead = _diagnosticData["totalAhead"] / _diagnosticData["numAhead"];
            _diagnosticsFile.WriteLine("Average frames ahead: {0}", averageAhead);
            _diagnosticsFile.Close();
        }
    }

    public override void RecordState(PlayerState ps)
    {
        if (_logging)
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

    public override void SetCurrentFrame(int frame)
    {
        if (_diagnostics)
        {
            int difference = _currentFrame - frame;
            _diagnosticData["totalDifference"] = difference;
            if (difference < 0)
            {
                _diagnosticData["numBehind"]++;
                _diagnosticData["totalBehind"] += difference;
            }
            else if (difference > 0)
            {
                _diagnosticData["numAhead"]++;
                _diagnosticData["totalAhead"] += difference;
            }
            else _diagnosticData["synchronised"]++;
            _diagnosticData["syncs"]++;
        }

        base.SetCurrentFrame(frame);
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
        file.Close();
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
