using System;
using Skyline.DataMiner.Analytics.GenericInterface;

[GQIMetaData(Name = "DateIsOlderThan")]
public class DateIsOlderThan : IGQIRowOperator, IGQIInputArguments, IGQIColumnOperator
{
    private readonly GQIColumnDropdownArgument _dateTimeColumnArg = new GQIColumnDropdownArgument("Date Time Column") { IsRequired = true , Types = new GQIColumnType[] { GQIColumnType.DateTime, GQIColumnType.Double } };
    private readonly GQIIntArgument _numberOfDaysArg = new GQIIntArgument("Number of days") { IsRequired = true };
    private readonly GQIStringColumn _validationColumn = new GQIStringColumn("Validation");

    private GQIColumn _dateTimeColumn;
    private int _numberOfDays;

    public GQIArgument[] GetInputArguments()
    {
        return new GQIArgument[] { _dateTimeColumnArg, _numberOfDaysArg };
    }

    public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
    {
        _dateTimeColumn = args.GetArgumentValue(_dateTimeColumnArg);
        _numberOfDays = args.GetArgumentValue(_numberOfDaysArg);

        return new OnArgumentsProcessedOutputArgs();
    }

    public void HandleColumns(GQIEditableHeader header)
    {
        header.AddColumns(_validationColumn);
    }

    public void HandleRow(GQIEditableRow row)
    {
        try
        {
            TimeSpan t = new TimeSpan(_numberOfDays, 0, 0, 0);

            // Start Time
            DateTime start;
            if (_dateTimeColumn.Type == GQIColumnType.DateTime)
            {
                start = row.GetValue<DateTime>(_dateTimeColumn);
            }
            else
            { // GQIColumnType.Double
                double startTimeAsdouble = row.GetValue<double>(_dateTimeColumn);
                start = DateTime.FromOADate(startTimeAsdouble).ToUniversalTime();
            }

            DateTime end = DateTime.UtcNow;
            String validation = (start + t < end) ? "No" : "Yes";

            row.SetValue(_validationColumn, validation);
        }
        catch (Exception)
        {
        }
    }
}