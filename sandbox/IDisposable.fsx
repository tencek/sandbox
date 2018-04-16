
open System

/// <summary>
/// A siple wrapper class for an external process / program running
/// Implements <see cref="System.IDisposable"/> - kills the process when released
/// </summary>
type ExternalProcess =
    { Process: System.Diagnostics.Process }
    interface IDisposable with
        member this.Dispose () =
            this.Process.Kill()
            this.Process.Close()
            this.Process.Dispose()
