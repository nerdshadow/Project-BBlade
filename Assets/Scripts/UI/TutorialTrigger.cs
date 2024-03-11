using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField]
    GameObject tutorialUIElement;
    [SerializeField]
    GameObject currentUIElement;
    private void OnTriggerEnter(Collider other)
    {
        PlayerStats player = other.GetComponent<PlayerStats>();
        if (player != null)
        {
            TrySpawnTutorial();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        PlayerStats player = other.GetComponent<PlayerStats>();
        if (player != null)
        {
            TryDespawnTutorial();
        }
    }
    void TrySpawnTutorial()
    {
        Transform potTransform = FindObjectOfType<GameplayMenuBehavior>().transform;
        if (potTransform == null)
            return;
        if (currentUIElement == null)
        {
            currentUIElement = Instantiate(tutorialUIElement, potTransform);
            currentUIElement.GetComponent<CanvasGroup>().alpha = 0f;
        }
        currentUIElement.SetActive(true);
        ChangeAlpha(currentUIElement.GetComponent<CanvasGroup>(), currentUIElement.GetComponent<CanvasGroup>().alpha, 1f);
    }
    void TryDespawnTutorial()
    {
        if (currentUIElement == null)
            return;
        ChangeAlpha(currentUIElement.GetComponent<CanvasGroup>(), currentUIElement.GetComponent<CanvasGroup>().alpha, 0f);
    }
    Coroutine changingAlpha;
    void ChangeAlpha(CanvasGroup _group, float _startvalue, float _endValue)
    {
        if (changingAlpha != null)
            StopCoroutine(changingAlpha);
        changingAlpha = StartCoroutine(ChangeAlpha(_group, _startvalue, _endValue, 1f));
    }
    IEnumerator ChangeAlpha(CanvasGroup _group, float _startAlpha, float _endAlpha, float _speed)
    {
        if (_group.alpha == _endAlpha)
            yield break;
        float currentAlpha = _startAlpha;
        if (currentAlpha < _endAlpha)
        {
            while (currentAlpha < _endAlpha)
            {
                currentAlpha += _speed * Time.deltaTime;
                _group.alpha = currentAlpha;
                yield return null;
            }
        }
        else
        {
            while (currentAlpha > _endAlpha)
            {
                currentAlpha -= _speed * Time.deltaTime;
                _group.alpha = currentAlpha;
                yield return null;
            }
        }
        _group.alpha = _endAlpha;
        if (_group.alpha <= 0)
            Destroy(_group.gameObject);
    }
}
