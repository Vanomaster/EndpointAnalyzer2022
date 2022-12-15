using Commands.Base;
using Dal;
using Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Queries.NonDatabase;

namespace Commands;

/// <inheritdoc />
public class AddOrUpdateGpParameterFromCsvFileCommand : CommandBase<List<FullGpParametersScvModel>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddOrUpdateEntityCommand"/> class.
    /// </summary>
    /// <param name="contextFactory">Context factory.</param>
    public AddOrUpdateGpParameterFromCsvFileCommand(IDbContextFactory<Context> contextFactory)
        : base(contextFactory)
    {
    }

    /// <inheritdoc/>
    protected override CommandResult ExecuteCore(List<FullGpParametersScvModel> fullGpParameterScvModels)
    {
        var gpParameters = new List<GpParameter>();
        var gpParameterRegistryParameters = new List<GpParameterRegistryParameter>();
        var gpParametersRecommendations = new List<GpParametersRecommendation>();
        foreach (var scvModel in fullGpParameterScvModels)
        {
            var gpParameter = new GpParameter
            {
                Name = scvModel.Name,
                Description = scvModel.Description,
            };

            var gpParameterRegistryParameter = new GpParameterRegistryParameter
            {
                KeyName = scvModel.KeyName,
                ParameterName = scvModel.ParameterName,
                GpParameter = gpParameter,
            };

            var gpParametersRecommendation = new GpParametersRecommendation
            {
                Value = scvModel.Value,
                Direction = scvModel.Direction,
                Rationale = scvModel.Rationale,
                Impact = scvModel.Impact,
                GpParameter = gpParameter,
            };

            gpParameters.Add(gpParameter);
            gpParameterRegistryParameters.Add(gpParameterRegistryParameter);
            gpParametersRecommendations.Add(gpParametersRecommendation);
        }

        Context.RemoveRange(Context.GpParameters);
        Context.RemoveRange(Context.GpParameterRegistryParameters);
        Context.RemoveRange(Context.GpParametersRecommendations);
        Context.AddRange(gpParameters);
        Context.AddRange(gpParameterRegistryParameters);
        Context.AddRange(gpParametersRecommendations);
        Context.SaveChanges();

        return GetSuccessfulResult();
    }
}