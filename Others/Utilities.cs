using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

// TODO: Refactor this class.

/// <summary>
/// Contain methods that search for objects in the scene.
/// </summary>
public class Utilities : MonoBehaviour
{
    // https://answers.unity.com/questions/460727/how-to-serialize-dictionary-with-unity-serializati.html
    // https://forum.unity.com/threads/finally-a-serializable-dictionary-for-unity-extracted-from-system-collections-generic.335797/
    // Add tags to identify state machines.
    [Serializable] 
    public class TaggedLayer
    {
        //[SerializeField] private List<string> 
        [SerializeField] private List<TaggedStateMachine> stateMachines; // List of state machines and states.
        [HideInInspector]
        [SerializeField] private AnimatorController animatorController; // Animator controller containing the state machines.

        public List<TaggedStateMachine> StateMachines
        {
            get { return stateMachines; }
            set { stateMachines = value; }
        }

        // Object holding the tag that can be modified in the inspector.
        [Serializable]
        public class TaggedStateMachine
        {
            [SerializeField] private string name, tag;
            [HideInInspector]
            [SerializeField] private int index, parentIndex;
            [HideInInspector]
            [SerializeField] private AnimatorState state;

            public TaggedStateMachine(string name)
            {
                this.name = name;
            }

            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            public string Tag
            {
                get { return tag; }
                set { tag = value; }
            }

            public int Index
            {
                get { return index; }
                set { index = value; }
            }

            public int ParentIndex
            {
                get { return parentIndex; }
                set { parentIndex = value; }
            }

            public AnimatorState State
            {
                get { return state; }
                set { state = value; }
            }
        }

        public TaggedLayer(AnimatorController animatorController, int layer)
        {
            this.animatorController = animatorController;
            stateMachines = new List<TaggedStateMachine>();
            AnimatorStateMachine stateMachine = this.animatorController.layers[layer].stateMachine;
            TaggedStateMachine rootStateMachine = new TaggedStateMachine(stateMachine.name);
            rootStateMachine.Index = stateMachines.Count;
            stateMachines.Add(rootStateMachine);
            StateMachinesDFS(stateMachine, rootStateMachine);
        }

        // Recursive depth-first search of the state machines.
        void StateMachinesDFS(AnimatorStateMachine stateMachine, TaggedStateMachine parentStateMachine)
        {
            ChildAnimatorStateMachine[] substateMachines = stateMachine.stateMachines;

            // Check if the current state machine contains any substate machines.
            if (substateMachines.Length > 0)
            {
                foreach (ChildAnimatorStateMachine substateMachine in substateMachines)
                {
                    TaggedStateMachine childStateMachine = new TaggedStateMachine(parentStateMachine.Name + "." + substateMachine.stateMachine.name);
                    childStateMachine.Index = stateMachines.Count;
                    childStateMachine.ParentIndex = parentStateMachine.Index;
                    stateMachines.Add(childStateMachine);
                    //Debug.Log("From child: " + childStateMachine.ParentIndex + " vs From parent: " + parentStateMachine.Index);
                    StateMachinesDFS(substateMachine.stateMachine, childStateMachine);
                }
            }

            // Check if the current state machine contains any states.
            ChildAnimatorState[] states = stateMachine.states;
            if (states.Length > 0)
            {
                foreach (ChildAnimatorState state in states)
                {
                    TaggedStateMachine childState = new TaggedStateMachine(parentStateMachine.Name + "." + state.state.name);
                    childState.Index = stateMachines.Count;
                    childState.ParentIndex = parentStateMachine.Index;
                    childState.State = state.state;
                    stateMachines.Add(childState);
                    //Debug.Log("From child: " + childState.ParentIndex + " vs From parent: " + parentStateMachine.Index);
                    //nameHashes.Add(Animator.StringToHash(childState.Name), childState);
                    //Debug.Log(nameHashes.Count);
                }
            }
        }

        // If a state machine doesn't have a tag, the substate machines will conserve theirs (replaced otherwise).
        public void TagStateMachines()
        {
            // Iterate through the state machines.
            for (int i = 1; i < stateMachines.Count; i++)
            {
                TaggedStateMachine currentStateMachine = stateMachines[i];
                TaggedStateMachine parentStateMachine = stateMachines[currentStateMachine.ParentIndex];
                if (parentStateMachine.Tag != "") currentStateMachine.Tag = parentStateMachine.Tag;
                if (currentStateMachine.State != null) currentStateMachine.State.tag = currentStateMachine.Tag;
                //Debug.Log("From child(" + currentStateMachine.Name +"): " + currentStateMachine.ParentIndex + " vs From Parent(" + parentStateMachine.Name + "): " + parentStateMachine.Index);
            }
        }
    }

    // https://answers.unity.com/questions/458207/copy-a-component-at-runtime.html
    // Copy a component from one game object to another.
    public static Component CopyComponent(Component original, GameObject destination)
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        // Copied fields can be restricted with BindingFlags
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy;
    }

    public static T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        const System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly;
        System.Reflection.PropertyInfo[] fields = type.GetProperties(flags);
        foreach (System.Reflection.PropertyInfo field in fields)
        {
            try
            {
                field.SetValue(copy, field.GetValue(original));
                Debug.Log(field);
                Debug.Log(field.GetValue(original));
            }
            catch (System.Exception e) { }
        }
        return copy as T;
    }

    // https://answers.unity.com/questions/179310/how-to-find-all-objects-in-specific-layer.html
    // Find all the game objects inside a layer.
    public static GameObject[] FindGameObjectsFromLayer(int layer)
    {
        var source = FindObjectsOfType<GameObject>();
        var copy = new List<GameObject>();
        for (var i = 0; i < source.Length; i++)
        {
            if (source[i].layer == layer)
            {
                copy.Add(source[i]);
            }
        }
        return copy.Count == 0 ? null : copy.ToArray();
    }
}
