namespace MagicCollectionHelper

module ErrorHandling =
    type Error =
        | InvalidArguments
        | NonExistingFile

    type ErrorMsgWithCode = { code: int; msg: string }

    module ErrorMsgWithCode =
        let create msg code = { code = code; msg = msg }

    /// Prints an error message to the console for a specific error and returns
    /// an error message code
    let handleError error =
        // Convert error to message and error code
        let msgWithCode =
            match error with
            | InvalidArguments -> "Usage: mch <filepath>", 1
            | NonExistingFile -> "Filepath invalid, file does not exist", 2
            ||> ErrorMsgWithCode.create

        printfn "%s" msgWithCode.msg
        msgWithCode.code
