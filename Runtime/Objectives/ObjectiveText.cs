using _S.Objectives;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ObjectiveText : MonoBehaviour
{

    ObjectiveUI _UIManager;
    Objective _objective;

    float _letterWaitTime;
    float _subObjectiveWaitTime;
    string _finalText;

    TextMeshProUGUI _textMesh;
    Animator _animator;

    private void OnEnable()
    {
        //_objectiveManager.Started += AddedToManager;
    }
    private void OnDisable()
    {
        //_objectiveManager.Started -= AddedToManager;
        if (_objective != null)
        {
            _objective.Ended -= CompletedObjective;
            EventObjective eventObjective = _objective as EventObjective;
            if (eventObjective != null)
            {
                eventObjective.Raised -= TextUpdated;
            }
        }
    }
    void Start()
    {
        if (true)
        {

        }
    }

    public void Initialise(ObjectiveUI UIManager, Objective objective, float letterWaitTime, float subObjectiveWaitTime)
    {
        _UIManager = UIManager;
        _objective = objective;
        _letterWaitTime = letterWaitTime;
        _subObjectiveWaitTime = subObjectiveWaitTime;
        _finalText = _objective.ObjectiveText;
        _textMesh = GetComponent<TextMeshProUGUI>();
        _animator = GetComponent<Animator>();
        objective.Ended += CompletedObjective;
        EventObjective eventObjective = objective as EventObjective;
        if (eventObjective != null)
        {
            eventObjective.Raised += TextUpdated;
        }
        SequenceObjective sequence = objective as SequenceObjective;
        if (sequence != null)
        {
            sequence.ConditionMet += TextUpdated;
        }
        StartCoroutine(WriteText());
    }

    public void CompletedObjective(Objective objective)
    {
        objective.Ended -= CompletedObjective;
        EventObjective eventObjective = _objective as EventObjective;
        if (eventObjective != null)
        {
            eventObjective.Raised -= TextUpdated;
        }
        SequenceObjective sequence = objective as SequenceObjective;
        if (sequence != null)
        {
            sequence.ConditionMet -= TextUpdated;
        }
        StopAllCoroutines();

        string _uncolouredText = "";
        int charsIndex = 0;
        bool inColouredText = false;
        while (true)
        {
            char writeChar = _finalText[charsIndex];
            if (writeChar == '<')
            {
                if (charsIndex + 4 < _finalText.Length)
                {
                    string subString = _finalText.Substring(charsIndex + 1, 5);
                    if (subString == "color" || subString == "/colo")
                    {
                        inColouredText = true;
                    }
                }
            }

            if (!inColouredText)
            {
                _uncolouredText = $"{_uncolouredText}{writeChar}";
            }
            if (writeChar == '>')
            {
                inColouredText = false;
            }
            charsIndex++;
            if (charsIndex == _finalText.Length) { break; }
        }
        _textMesh.text = _uncolouredText;

        string triggerName = "";
        switch (_objective.State)
        {
            case Objective.CompletionState.Failed:
                triggerName = "Failed";
                break;
            case Objective.CompletionState.Succeeded:
                triggerName = "Succeeded";
                break;
            default:
                triggerName = "Inactive";
                break;
        }
        StartCoroutine(DelayedAnimation(_subObjectiveWaitTime * objective.EndDepth, triggerName));
    }

    public IEnumerator DelayedAnimation(float waitTime, string triggerName)
    {
        yield return new WaitForSeconds(waitTime);
        _animator.SetTrigger(triggerName);
        _UIManager.CompletedObjective(transform, _objective);
    }
    public IEnumerator WriteText()
    {
        string currentText = "";
        _textMesh.text = "";
        int charsIndex = 0;
        int finalChars = _objective.ObjectiveText.Length;
        bool running = true;
        if (_objective.Parent is SequenceObjective)
        {
            yield return new WaitForSeconds(_subObjectiveWaitTime);
        }
        while (running)
        {
            yield return new WaitForSeconds(_letterWaitTime);
            char writeChar = ' ';
            bool inAngleBracket = false;
            while (writeChar == ' ' || inAngleBracket)
            {
                writeChar = _finalText[charsIndex];
                if (writeChar == '<')
                {
                    inAngleBracket = true;
                }
                if (writeChar == '>')
                {
                    inAngleBracket = false;
                }
                currentText = _finalText.Substring(0, charsIndex + 1);

                if (charsIndex == finalChars - 1)
                {
                    _textMesh.text = currentText;
                    running = false;
                    yield break;
                }
                _textMesh.text = $"{currentText}_";
                charsIndex++;
            }
        }
    }

    void TextUpdated(Objective objective)
    {
        if (_textMesh.text.Equals(_finalText))
        {
            _textMesh.text = objective.ObjectiveText;
        }
        _finalText = objective.ObjectiveText;
    }
}
