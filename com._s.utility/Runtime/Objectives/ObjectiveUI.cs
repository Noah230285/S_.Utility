using _S.Objectives;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class ObjectiveUI : MonoBehaviour
{
    [SerializeField] GameObject _objectiveTextPrefab;
    [SerializeField] Transform _objectiveTextContainer;
    [SerializeField] float _letterWaitTime;
    [SerializeField] AnimationCurve _transitionCurve;
    [SerializeField] float _transitionTime;
    [SerializeField] float _transitionDelay;
    [SerializeField] float _subObjectiveOffset;
    [SerializeField] float _subObjectiveWaitTime;
    [SerializeField] ObjectiveManager _objectiveManager;

    List<float> _heights = new List<float>();
    List<RectTransform> _objectiveRects = new List<RectTransform> { };
    List<Objective> _registeredObjectives = new List<Objective> { };

    private void Awake()
    {
        _objectiveManager.objectives.ForEach((t) => RegisterObjective(t.RootObjective));
    }
    private void OnEnable()
    {
        _objectiveManager.AddedObjective += RegisterObjectiveTree;
    }
    private void OnDisable()
    {
        _objectiveManager.AddedObjective -= RegisterObjectiveTree;
        foreach (var objective in _registeredObjectives)
        {
            objective.Started -= AddObjectiveText;
        }
        _registeredObjectives.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    void RegisterObjectiveTree(ObjectiveTree tree)
    {
        RegisterObjective(tree.RootObjective);
    }

    void RegisterObjective(Objective objective)
    {
        if (!objective.ObjectiveText.Equals(""))
        {
            if (objective.State == Objective.CompletionState.InProgress)
            {
                AddObjectiveText(objective);
            }
            else if (objective.State == Objective.CompletionState.Inactive)
            {
                objective.Started += AddObjectiveText;
            }
        }
        ConditionalObjective conditional = objective as ConditionalObjective;
        if (conditional != null)
        {
            foreach (var condition in conditional.Conditions)
            {
                RegisterObjective(condition);
            }
        }
        RootObjective root = objective as RootObjective;
        if (root != null)
        {
            RegisterObjective(root.Child);
        }
    }

    public void RemoveObjective(Objective objective)
    {
        objective.Started -= AddObjectiveText;
        _registeredObjectives.Remove(objective);
        ConditionalObjective conditional = objective as ConditionalObjective;
        if (conditional != null)
        {
            foreach (var condition in conditional.Conditions)
            {
                if (_registeredObjectives.Contains(condition))
                {
                    RemoveObjective(condition);
                }
            }
        }
        RootObjective root = objective as RootObjective;
        if (root != null)
        {
            if (_registeredObjectives.Contains(root.Child))
            {
                RemoveObjective(root.Child);
            }
        }
    }

    public void AddObjectiveText(Objective objective)
    {
        int placeIndex = 0;
        for (int i = 0; i < _registeredObjectives.Count; i++)
        {
            if (_registeredObjectives[i].ExecutionOrder > objective.ExecutionOrder)
            {
                placeIndex = i - 1;
                break;
            }
            if (i == _registeredObjectives.Count - 1)
            {
                placeIndex = i + 1;
            }
        }
        var newText = Instantiate(_objectiveTextPrefab, _objectiveTextContainer);
        var rect = newText.GetComponent<RectTransform>();
        float textHeight = rect.rect.height * objective.ObjectiveText.Split('\n').Length;
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textHeight);
        newText.GetComponent<TextMeshProUGUI>().margin += new Vector4(objective.StartDepth * _subObjectiveOffset, 0,0,0);

        float previousHeights = 0;
        for (int i = 0; i < placeIndex; i++)
        {
            previousHeights += _heights[i];

        }
        rect.anchoredPosition += Vector2.down * previousHeights;
        if (placeIndex == 0 || placeIndex == _registeredObjectives.Count)
        {
            _registeredObjectives.Add(objective);
            _objectiveRects.Add(rect);
            _heights.Add(textHeight);
        }
        else
        {
            _registeredObjectives.Insert(placeIndex, objective);
            _objectiveRects.Insert(placeIndex, rect);
            _heights.Insert(placeIndex, textHeight);
            newText.transform.SetSiblingIndex(placeIndex);
            StartCoroutine(MoveDown(newText.transform, objective));
        }
        var objectiveTextBehaviour = newText.GetComponent<ObjectiveText>();
        objectiveTextBehaviour.Initialise(this, objective, _letterWaitTime, _subObjectiveWaitTime);
    }

    public void CompletedObjective(Transform transform, Objective objective)
    {
        StartCoroutine(MoveUp(transform, objective));
    }
    IEnumerator MoveUp(Transform destroyedTransform, Objective objective)
    {
        int childIndex = destroyedTransform.GetSiblingIndex();
        float intialHeight = _heights[childIndex];
        float currentHeight = intialHeight;
        float time = 0;
        RectTransform rect = destroyedTransform.GetComponent<RectTransform>();
        yield return new WaitForSeconds(_transitionDelay);
        while (true)
        {

            float previousHeight = currentHeight;
            currentHeight = Mathf.Lerp(intialHeight, 0, _transitionCurve.Evaluate(time));
            _heights[destroyedTransform.GetSiblingIndex()] = currentHeight;
            for (int i = _objectiveRects.Count - 1; i >= 0; i--)
            {
                if (_registeredObjectives[i].ExecutionOrder <= objective.ExecutionOrder)
                {
                    Debug.Log("exit");
                    break;
                }
                Debug.Log(currentHeight);
                _objectiveRects[i].anchoredPosition -= new Vector2(0, currentHeight - previousHeight);
            }
            yield return null;
            time += Time.deltaTime / _transitionTime;
            if (time >= 1)
            {
                int index = _objectiveRects.IndexOf(rect);
                _objectiveRects.Remove(rect);
                _heights.RemoveAt(index);
                RemoveObjective(objective);
                Destroy(destroyedTransform.gameObject);
                yield break;
            }
        }
    }

    IEnumerator MoveDown(Transform addedTransform, Objective objective)
    {
        int childIndex = addedTransform.GetSiblingIndex();
        float toHeight = _heights[childIndex];
        float currentHeight = toHeight;
        float time = 0;
        RectTransform rect = addedTransform.GetComponent<RectTransform>();
        yield return new WaitForSeconds(_transitionDelay);
        while (true)
        {
            float previousHeight = currentHeight;
            currentHeight = Mathf.Lerp(toHeight, 0, _transitionCurve.Evaluate(time));
            for (int i = _objectiveRects.Count - 1; i >= 0; i--)
            {
                if (_registeredObjectives[i].ExecutionOrder <= objective.ExecutionOrder)
                {
                    break;
                }
                _objectiveRects[i].anchoredPosition += new Vector2(0, currentHeight - previousHeight);
            }
            yield return null;
            time += Time.deltaTime / _transitionTime;
            if (time >= 1)
            {
                yield break;
            }
        }
    }
}
