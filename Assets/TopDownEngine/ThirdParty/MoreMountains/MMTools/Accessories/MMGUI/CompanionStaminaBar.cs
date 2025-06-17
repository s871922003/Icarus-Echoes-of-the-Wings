using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
#if MM_UI
using UnityEngine.UI;
#endif

namespace MoreMountains.Tools
{
    /// <summary>
    /// Add this component to an object and it will show a stamina bar above it.
    /// This is similar to MMHealthBar but designed for CompanionStamina.
    /// </summary>
    [AddComponentMenu("More Mountains/Tools/GUI/Companion Stamina Bar")]
    public class CompanionStaminaBar : MonoBehaviour
    {
#if MM_UI
		public enum BarTypes { Prefab, Drawn, Existing }
		public enum TimeScales { UnscaledTime, Time }

		[Tooltip("whether the bar uses a prefab or is drawn automatically")]
		public BarTypes BarType = BarTypes.Drawn;
		[Tooltip("defines whether the bar works on scaled or unscaled time")]
		public TimeScales TimeScale = TimeScales.UnscaledTime;

		[Header("Prefab")] 
		public MMProgressBar BarPrefab;
		[Header("Existing")] 
		public MMProgressBar TargetProgressBar;

		[Header("Drawn Settings")]
		public Vector2 Size = new Vector2(1f, 0.2f);
		public Vector2 BackgroundPadding = new Vector2(0.01f, 0.01f);
		public Vector3 InitialRotationAngles;
		public Gradient ForegroundColor;
		public Gradient DelayedColor;
		public Gradient BorderColor;
		public Gradient BackgroundColor;
		public string SortingLayerName = "UI";
		public float Delay = 0.5f;
		public bool LerpFrontBar = true;
		public float LerpFrontBarSpeed = 15f;
		public bool LerpDelayedBar = true;
		public float LerpDelayedBarSpeed = 15f;
		public bool BumpScaleOnChange = true;
		public float BumpDuration = 0.2f;
		public AnimationCurve BumpAnimationCurve = AnimationCurve.Constant(0, 1, 1);

		[Tooltip("how the bar follows its target")]
		public MMFollowTarget.UpdateModes FollowTargetMode = MMFollowTarget.UpdateModes.LateUpdate;
		public bool FollowRotation = false;
		public bool FollowScale = true;
		public bool NestDrawnBar = false;
		public bool Billboard = false;

		[Header("Display Settings")]
		public bool AlwaysVisible = true;
		public float DisplayDurationOnChange = 1f;
		public bool HideBarAtZero = true;
		public float HideBarAtZeroDelay = 1f;
		public GameObject InstantiatedOnEmpty;
		public Vector3 BarOffset = new Vector3(0f, 1f, 0f);

		[Header("Test")]
		public float TestMin = 0f;
		public float TestMax = 100f;
		public float TestCurrent = 50f;
		[MMInspectorButton("TestUpdate")]
		public bool TestUpdateButton;

		protected MMProgressBar _progressBar;
		protected MMFollowTarget _followTransform;
		protected float _lastShowTimestamp = 0f;
		protected bool _showBar = false;
		protected Image _backgroundImage, _borderImage, _foregroundImage, _delayedImage;
		protected bool _finalHideStarted = false;

		protected virtual void Awake() => Initialization();

		protected virtual void OnEnable()
		{
			_finalHideStarted = false;
			SetInitialActiveState();
		}

		public virtual void SetInitialActiveState()
		{
			if (!AlwaysVisible && (_progressBar != null)) { ShowBar(false); }
		}

		public virtual void ShowBar(bool state) => _progressBar.gameObject.SetActive(state);
		public virtual bool BarIsShown() => _progressBar.gameObject.activeInHierarchy;

		public virtual void Initialization()
		{
			_finalHideStarted = false;
			if (_progressBar != null) { ShowBar(AlwaysVisible); return; }

			switch (BarType)
			{
				case BarTypes.Prefab:
					if (BarPrefab == null) { Debug.LogWarning("Missing BarPrefab"); return; }
					_progressBar = Instantiate(BarPrefab, transform.position + BarOffset, transform.rotation);
					SceneManager.MoveGameObjectToScene(_progressBar.gameObject, this.gameObject.scene);
					_progressBar.transform.SetParent(this.transform);
					break;
				case BarTypes.Drawn:
					DrawBar();
					UpdateDrawnColors();
					break;
				case BarTypes.Existing:
					_progressBar = TargetProgressBar;
					break;
			}

			if (!AlwaysVisible) { ShowBar(false); }
			_progressBar?.SetBar(100f, 0f, 100f);
		}

		protected virtual void DrawBar()
		{
			GameObject go = new GameObject("StaminaBar|" + gameObject.name);
			SceneManager.MoveGameObjectToScene(go, gameObject.scene);
			if (NestDrawnBar) { go.transform.SetParent(transform); }

			_progressBar = go.AddComponent<MMProgressBar>();
			_followTransform = go.AddComponent<MMFollowTarget>();
			_followTransform.Target = transform;
			_followTransform.Offset = BarOffset;
			_followTransform.FollowRotation = FollowRotation;
			_followTransform.FollowScale = FollowScale;
			_followTransform.UpdateMode = FollowTargetMode;

			Canvas canvas = go.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.WorldSpace;
			canvas.transform.localScale = Vector3.one;
			canvas.GetComponent<RectTransform>().sizeDelta = Size;
			if (!string.IsNullOrEmpty(SortingLayerName)) canvas.sortingLayerName = SortingLayerName;

			GameObject container = new GameObject("StaminaBarContainer");
			container.transform.SetParent(go.transform);

			// Border
			_borderImage = CreateImage("Border", container.transform, Size);
			// Background
			_backgroundImage = CreateImage("Background", container.transform, Size - BackgroundPadding * 2);
			// Delayed
			_delayedImage = CreateImage("Delayed", container.transform, Size - BackgroundPadding * 2);
			// Foreground
			_foregroundImage = CreateImage("Foreground", container.transform, Size - BackgroundPadding * 2);
			_foregroundImage.color = ForegroundColor.Evaluate(1);

			if (Billboard)
			{
				MMBillboard billboard = _progressBar.gameObject.AddComponent<MMBillboard>();
				billboard.NestObject = !NestDrawnBar;
			}

			_progressBar.ForegroundBar = _foregroundImage.transform;
			_progressBar.DelayedBarDecreasing = _delayedImage.transform;
			_progressBar.DecreasingDelay = Delay;
			_progressBar.LerpForegroundBar = LerpFrontBar;
			_progressBar.LerpDecreasingDelayedBar = LerpDelayedBar;
			_progressBar.LerpForegroundBarSpeedIncreasing = LerpFrontBarSpeed;
			_progressBar.LerpDecreasingDelayedBarSpeed = LerpDelayedBarSpeed;
			_progressBar.BumpScaleOnChange = BumpScaleOnChange;
			_progressBar.BumpDuration = BumpDuration;
			_progressBar.BumpScaleAnimationCurve = BumpAnimationCurve;
			_progressBar.TimeScale = (TimeScale == TimeScales.Time) ? MMProgressBar.TimeScales.Time : MMProgressBar.TimeScales.UnscaledTime;
			container.transform.localEulerAngles = InitialRotationAngles;
			_progressBar.Initialization();
		}

		protected Image CreateImage(string name, Transform parent, Vector2 size)
		{
			GameObject go = new GameObject(name);
			go.transform.SetParent(parent);
			Image img = go.AddComponent<Image>();
			img.rectTransform.sizeDelta = size;
			img.rectTransform.anchoredPosition = -size / 2;
			img.rectTransform.pivot = Vector2.zero;
			return img;
		}

		protected virtual void Update()
		{
			if (_progressBar == null || _finalHideStarted) return;
			UpdateDrawnColors();
			if (AlwaysVisible) return;

			if (_showBar)
			{
				ShowBar(true);
				float t = (TimeScale == TimeScales.UnscaledTime) ? Time.unscaledTime : Time.time;
				if (t - _lastShowTimestamp > DisplayDurationOnChange) _showBar = false;
			}
			else if (BarIsShown())
			{
				ShowBar(false);
			}
		}

		protected virtual void UpdateDrawnColors()
		{
			if (BarType != BarTypes.Drawn || _progressBar.Bumping) return;
			if (_borderImage != null) _borderImage.color = BorderColor.Evaluate(_progressBar.BarProgress);
			if (_backgroundImage != null) _backgroundImage.color = BackgroundColor.Evaluate(_progressBar.BarProgress);
			if (_delayedImage != null) _delayedImage.color = DelayedColor.Evaluate(_progressBar.BarProgress);
			if (_foregroundImage != null) _foregroundImage.color = ForegroundColor.Evaluate(_progressBar.BarProgress);
		}

		public virtual void UpdateBar(float current, float min, float max, bool show)
		{
			if (!AlwaysVisible && show)
			{
				_showBar = true;
				_lastShowTimestamp = (TimeScale == TimeScales.UnscaledTime) ? Time.unscaledTime : Time.time;
			}

			if (_progressBar != null)
			{
				_progressBar.UpdateBar(current, min, max);
				if (HideBarAtZero && _progressBar.BarTarget <= 0) StartCoroutine(FinalHideBar());
				if (BumpScaleOnChange) _progressBar.Bump();
			}
		}

		protected virtual IEnumerator FinalHideBar()
		{
			_finalHideStarted = true;
			if (InstantiatedOnEmpty != null)
			{
				GameObject fx = Instantiate(InstantiatedOnEmpty, transform.position + BarOffset, transform.rotation);
				SceneManager.MoveGameObjectToScene(fx, gameObject.scene);
			}
			if (HideBarAtZeroDelay == 0) { _showBar = false; ShowBar(false); yield break; }
			_progressBar.HideBar(HideBarAtZeroDelay);
			yield return null;
		}

		protected virtual void TestUpdate() => UpdateBar(TestCurrent, TestMin, TestMax, true);
#endif
    }
}
