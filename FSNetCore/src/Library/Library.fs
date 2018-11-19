namespace Ncr.Devices.PointOfService

module CustomerDisplayCli = 
    open System.Runtime.InteropServices
    open Microsoft.FSharp.Core

    type Status = 
        | Offline = 0 
        | Online = 1

    type CommandType = 
            | AddCondiment = 0
            | AddFood = 1
            | AddPiece = 2
            | AddSide = 3
            | AddTextItem = 4
            | AddWithoutCondiment = 5
            | CancelItem = 6
            | CancelTran = 7
            | ClearTotal = 8
            | ComboFlavor = 9
            | ComboItem = 10
            | ConsolidateTran = 11
            | Destination = 12
            | DisplaySelfTest = 13
            | DoEod = 14
            | FoodItem = 15
            | GenericMessage = 16
            | Message = 17
            | ModifyCombo = 18
            | ModifyCondiment = 19
            | ModifyFlavor = 20
            | ModifyFood = 21
            | Piece = 22
            | PizzaItem = 23
            | RecallTran = 24
            | RefundSalesTran = 25
            | RegisterClosed = 26
            | RegisterOpen = 27
            | RemoveComboSubitems = 28
            | RemoveCondiment = 29
            | RemoveFood = 30
            | RemovePiece = 31
            | RemoveSide = 32
            | RetrieveAndRefundFinalized = 33
            | RetrieveAndRefundTran = 34
            | SideItem = 35
            | StartTran = 36
            | StoreTran = 37
            | SubTotalTran = 38
            | TenderTran = 39
            | TotalTran = 40

    type StatusChangedCallBack = delegate of Status -> unit
    
    [<CLIMutable>]
    [<StructLayout(LayoutKind.Sequential)>]
    type Command = {
        TransactionNumber:int
        CommandType:CommandType
        LiteralString:string
        CodePage:int
        EmployeeID:int64
        TaxAmount:int64
        SubTotalAmount:int64
        TotalAmount:int64
        CreditTotal:int64
        DiscountTotal:int64
        DebitTotal:int64
        ServiceChargeTotal:int64
        ChangeAmount:int64
        DisplayTime:int
        Balance:int64
        ItemNumber:int
        ItemID:int64
        ItemType:int64
        Quantity:float
        UnitPrice:int64
        Modifier1ID:int64
        Modifier2ID:int64
        Modifier3ID:int64
        ExtendedPrice:int64
        FormattedDisplay:string
        ParentItemNumber:int
        isFreeItem:bool
        PumpNumber:int
        isFuelItem:bool
        isChangeTender:bool
        ShortDescription:string
    }

    let CreateEmptyCommand commandType = 
        {
            TransactionNumber = 0
            CommandType = commandType
            LiteralString = ""
            CodePage = 0
            EmployeeID = 0L
            TaxAmount = 0L
            SubTotalAmount = 0L
            TotalAmount = 0L
            CreditTotal = 0L
            DiscountTotal = 0L
            DebitTotal = 0L
            ServiceChargeTotal = 0L
            ChangeAmount = 0L
            DisplayTime = 0
            Balance = 0L
            ItemNumber = 0
            ItemID = 0L
            ItemType = 0L
            Quantity = 0.0
            UnitPrice = 0L
            Modifier1ID = 0L
            Modifier2ID = 0L
            Modifier3ID = 0L
            ExtendedPrice = 0L
            FormattedDisplay = ""
            ParentItemNumber = 0
            isFreeItem = false
            PumpNumber = 0
            isFuelItem = false
            isChangeTender = false
            ShortDescription = ""
        }

    [<DllImport(@"C:\dev\rpos\PCS_VBL_2018.2\6.1\bin\NT-VS10\Debug\CustomerDisplayAPI.dll", EntryPoint="SendGenericCommand", CallingConvention = CallingConvention.StdCall)>]
    extern bool SendGenericCommand(Command command)

    [<DllImport(@"C:\dev\rpos\PCS_VBL_2018.2\6.1\bin\NT-VS10\Debug\CustomerDisplayAPI.dll", EntryPoint="SetStatusNotificationByCallback", CallingConvention = CallingConvention.StdCall)>]
    extern bool SetStatusNotificationByCallback(StatusChangedCallBack callback)

    [<DllImport(@"C:\dev\rpos\PCS_VBL_2018.2\6.1\bin\NT-VS10\Debug\CustomerDisplayAPI.dll", EntryPoint="CDGreater", CallingConvention = CallingConvention.StdCall)>]
    extern bool CDGreater(int a, int b)
