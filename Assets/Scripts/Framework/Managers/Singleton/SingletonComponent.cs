using UnityEngine;
using System.Collections;

namespace Game.Singleton
{
	/// <summary>
	/// Gets a static instance of the Component that extends this class and makes it accessible through the Instance property.
	/// </summary>
	public class SingletonComponent<T> : MonoBehaviour where T : Component
	{
		#region Member Variables

		private static T instance;

		private bool isInitialized;

		[SerializeField] public bool dontDestroyOnLoad = true;

		#endregion

		#region Properties

		public static T Instance
		{
			get
			{
				// If the instance is null then either Instance was called to early or this object is not active.
				if (instance == null)
				{
					instance = GameObject.FindObjectOfType<T>();
				}

				if (instance == null)
				{
					var singletonObject = new GameObject();
					instance = singletonObject.AddComponent<T>();
					singletonObject.name = typeof(T).ToString() + " (Singleton)";
				}

				return instance;
			}
		}

		#endregion

		#region Unity Methods

		protected virtual void Awake()
		{
			SetInstance();
		}

		#endregion

		#region Public Methods

		public static bool Exists()
		{
			return instance != null;
		}

		public bool SetInstance()
		{
			if (instance != null && instance != gameObject.GetComponent<T>())
			{
				Debug.LogWarning("[SingletonComponent] Instance already set for type " + typeof(T));
				return false;
			}

			instance = gameObject.GetComponent<T>();
			if(dontDestroyOnLoad)
			{
				DontDestroyOnLoad ( gameObject );
			}
			
			return true;
		}

		#endregion
	}
}
