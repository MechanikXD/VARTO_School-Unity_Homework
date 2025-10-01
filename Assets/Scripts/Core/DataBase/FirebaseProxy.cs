using Core.Behaviour.Singleton;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;

namespace Core.DataBase
{
    public class FirebaseProxy : SingletonBase<FirebaseProxy>
    {
        private const string DATABASE_LINK =
            "https://unityhomework-8a616-default-rtdb.europe-west1.firebasedatabase.app/";

        public static DatabaseReference DB { get; private set; }
        public static string UId { get; private set; }
        public bool IsInitialized { get; private set; }

        private async UniTask InitAsync()
        {
            IsInitialized = false;
            var dependency = await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask();

            if (dependency != DependencyStatus.Available)
            {
                Debug.Log("Firebase: " + dependency);
                return;
            }

            var auth = FirebaseAuth.DefaultInstance;
            if (auth.CurrentUser == null)
            {
                await auth.SignInAnonymouslyAsync().AsUniTask();
            }
            UId = auth.CurrentUser!.UserId;

            var app = FirebaseApp.DefaultInstance;
            var db = FirebaseDatabase.GetInstance(app, DATABASE_LINK);
            
            db.SetPersistenceEnabled(false);
            DB = db.RootReference;
            IsInitialized = true;
        }
        
        public async UniTask WritePositionAsync(Vector3 newPosition)
        {
            if (!IsInitialized) await InitAsync();
            var positionNode = DB.Child("users").Child(UId).Child("stats").Child("position");
            await positionNode.SetValueAsync(Vector3ToString(newPosition)).AsUniTask();
        }

        public async UniTask<Vector3> GetPositionAsync(Vector3 defaultValue)
        {
            if (!IsInitialized) await InitAsync();
            var position = await DB.Child("users").Child(UId).Child("stats").Child("position")
                .GetValueAsync().AsUniTask();

            return position.Exists ? StringToVector3((string)position.Value) : defaultValue;
        }

        private static string Vector3ToString(Vector3 vector)
        {
            return $"{vector.x}:{vector.y}:{vector.z}";
        }

        private static Vector3 StringToVector3(string text)
        {
            var numbers = text.Split(':');
            return new Vector3(float.Parse(numbers[0]), float.Parse(numbers[1]),
                float.Parse(numbers[2]));
        }
    }
}