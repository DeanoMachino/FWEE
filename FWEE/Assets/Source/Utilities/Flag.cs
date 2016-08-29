using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Templated class to act as a flagged enum container. Allows the user to store
// multiple enum values within one variable and perform generic operations on
// them.
public class Flag<T> {
    private List<T> m_flags;

    public Flag(params T[] flags) {
        Set(flags);
    }

    // Replace the current flags with new flags.
    public void Set(params T[] flags) {
        // Clear the objcet flag list.
        m_flags = new List<T>();

        // Add each new flag to the object flag list.
        foreach (T f in flags) {
            Add(f);
        }
    }

    // Adds new flags to the object.
    public void Add(params T[] flags) {
        foreach (T f in flags) {
            if (!HasFlag(f)) {
                m_flags.Add(f);
            }
        }
    }

    // Remove a flag from the object.
    public void Remove(T flag) {
        if (HasFlag(flag)) {
            m_flags.Remove(flag);
        }
    }

    // Returns a list of the object flags.
    public List<T> Get() {
        return m_flags;
    }

    // Check whether the object has a certain flag.
    public bool HasFlag(T flag) {
        foreach (T f in m_flags) {
            if (f.Equals(flag))
                return true;
        }

        return false;
    }

    // Check whether the object contains all of a list of flags.
    public bool HasAllFlags(params T[] flags) {
        foreach (T f in flags) {
            if (!HasFlag(f))
                return false;
        }

        return true;
    }

    // Check whether the object contains at least one of a list of flags.
    public bool HasSomeFlags(params T[] flags) {
        foreach (T f in flags) {
            if(HasFlag(f))
                return true;
        }

        return false;
    }

    // WARNING: Untested, possibly will not return a correct result. -Dean
    // Checks whether the object contains only the specified flags (but not necessarily all).
    public bool HasSomeFlagsOnly(params T[] flags) {
        foreach (T mf in m_flags) {
            foreach (T f in flags) {
                if (mf.Equals(f))
                    continue;

                return false;
            }
        }

        return true;
    }

    // Chceks whether the object contains only the specified flags (and all of them).
    public bool HasAllFlagsOnly(params T[] flags) {
        if(m_flags.Except(flags).Any())
            return false;

        return true;
    }

    public bool Is(params T[] flags) {
        if (flags.Length == m_flags.Count) {
            foreach (T f in flags) {
                if (HasFlag(f))
                    continue;

                return false;
            }

            return true;
        }

        return false;
    }

    // Override ToString()
    public override string ToString() {
        string str = typeof(T).Name.ToString() + " Flags: ";
        foreach (T f in m_flags) {
            str += f.ToString() + " | ";
        }

        str = str.Substring(0, str.Length - 3);
        return str;
    }

}
