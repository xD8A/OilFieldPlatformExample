#pragma warning disable S125
using System.Diagnostics;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OilFieldPlatform.Calculation.Core.Models;
using OilFieldPlatform.Domain.Entities.Calculation;
using OilFieldPlatform.Domain.Entities.Calculation.Data;
using OilFieldPlatform.Domain.Entities.Common;
using OilFieldPlatform.Infrastructure.Extensions;

namespace OilFieldPlatform.Calculation.Core.Mapping;

/// <summary>AutoMapper-профиль для маппинга между Domain-сущностями и Core-моделями расчётного проекта.</summary>
public class ProjectProfile : Profile
{
    private readonly ILogger _logger;

    /// <summary>Конструктор. Настраивает все маппинги.</summary>
    /// <param name="logger">Логгер (опционально).</param>
    public ProjectProfile(ILogger<ProjectProfile>? logger = null)
    {
        _logger = (ILogger?)logger ?? NullLogger.Instance;

        _logger.LogDebug("Configuring AutoMapper mappings for ProjectProfile");

        _logger.LogDebug("Mapping: OilFieldEntity → OilField");

        CreateMap<OilFieldEntity, OilFieldModel>()
            .ConstructUsing((src, _) =>
            {
                Debug.Assert(src.Id is not null, "OilFieldEntity.Id should not be null when mapping to OilField");
                return new OilFieldModel(src.Name, (long)src.Id);
            })
            .ForAllMembers(o => o.Ignore());

        _logger.LogDebug("Mapping: DevTargetEntity → DevTarget");
        CreateMap<DevTargetEntity, DevTargetModel>()
            .ConstructUsing((src, _) =>
            {
                Debug.Assert(src.Id is not null, "DevTargetEntity.Id should not be null when mapping to DevTarget");
                return new DevTargetModel(src.Name, (long)src.Id);
            })
            .ForAllMembers(o => o.Ignore());

        _logger.LogDebug("Mapping: CalcProjectEntity → Project");
        CreateMap<CalcProjectEntity, ProjectModel>()
            .ConstructUsing((src, ctx) => new ProjectModel(
                ctx.Mapper.Map<OilFieldModel>(src.OilField),
                ctx.Mapper.Map<DevTargetModel>(src.DevTarget),
                null,
                src.Id))
            .AfterMap((src, dest, ctx) =>
            {
                _logger.LogDebug("Mapping water samples for project Id: {Id}", src.Id);
                if (ctx.TryGetItems(out var items))
                    items["ParentProject"] = dest;
                foreach (var ws in src.WaterSamples.Select(ws => ctx.Mapper.Map<WaterSampleModel>(ws)))
                    dest.AddWaterSample(ws);
            })
            .ForAllMembers(o => o.Ignore());

        _logger.LogDebug("Mapping: CalcWaterSampleEntity → WaterSample");
        CreateMap<CalcWaterSampleEntity, WaterSampleModel>()
            .ConstructUsing((src, ctx) =>
            {
                var parentProject = ctx.TryGetItems(out var items) && items.TryGetValue("ParentProject", out var p)
                    ? (ProjectModel)p
                    : ctx.Mapper.Map<ProjectModel>(src.Project);

                var ws = new WaterSampleModel(
                    parentProject,
                    src.SampledAt,
                    src.WaterType,
                    src.ClusterStationName,
                    src.WellName,
                    src.SourceSample?.Id,
                    src.Id);

                _logger.LogDebug("Mapping CalcWaterSampleEntity Id: {Id} → WaterSample", src.Id);

                return ws;
            })
            .IgnoreAllMembers()
            .ForMember(d => d.Chloride, o => o.MapFrom(s => s.Chloride))
            .ForMember(d => d.Carbonate, o => o.MapFrom(s => s.Carbonate))
            .ForMember(d => d.Bicarbonate, o => o.MapFrom(s => s.Bicarbonate))
            .ForMember(d => d.Sulfate, o => o.MapFrom(s => s.Sulfate))
            .ForMember(d => d.Calcium, o => o.MapFrom(s => s.Calcium))
            .ForMember(d => d.Magnesium, o => o.MapFrom(s => s.Magnesium))
            .ForMember(d => d.Sodium, o => o.MapFrom(s => s.Sodium))
            .AfterMap((src, dest, ctx) =>
            {
                if (src.Equivalent is not null && dest.Equivalent is not null)
                {
                    _logger.LogDebug("Mapping equivalent record for WaterSample Id: {Id}", src.Id);

                    dest.Equivalent.Chloride = src.Equivalent.Chloride;
                    dest.Equivalent.Carbonate = src.Equivalent.Carbonate;
                    dest.Equivalent.Bicarbonate = src.Equivalent.Bicarbonate;
                    dest.Equivalent.Sulfate = src.Equivalent.Sulfate;
                    dest.Equivalent.Calcium = src.Equivalent.Calcium;
                    dest.Equivalent.Magnesium = src.Equivalent.Magnesium;
                    dest.Equivalent.Sodium = src.Equivalent.Sodium;

                }
            });

        _logger.LogDebug("Mapping: WaterSample → CalcWaterSampleEntity");
        CreateMap<WaterSampleModel, CalcWaterSampleEntity>()
            .ConstructUsing((src, ctx) =>
            {
                if (!ctx.Items.TryGetValue("ParentProjectEntity", out var parentObj)
                    || parentObj is not CalcProjectEntity parent)
                {
                    _logger.LogError("ParentProjectEntity not passed in Items when mapping WaterSample");
                    Debug.Assert(false, "Pass ParentProjectEntity in Items when mapping WaterSample.");
                    return null!;
                }

                if (src.Id is not null)
                {
                    _logger.LogDebug("Loading existing CalcWaterSampleEntity by Id: {SampleId}", src.Id);
                    if (!ctx.Items.TryGetValue("Session", out var sessionObj)
                        || sessionObj is not NHibernate.ISession session)
                    {
                        _logger.LogError("Session not passed in Items when mapping WaterSample");
                        Debug.Assert(false, "Pass Session as NHibernate.ISession in Items when mapping WaterSample.");
                        return null!;
                    }
                    return session.Get<CalcWaterSampleEntity>(src.Id.Value);
                }
                _logger.LogDebug("Creating new CalcWaterSampleEntity");
                return new CalcWaterSampleEntity(parent, src.SampledAt, src.WaterType, src.ClusterStationName, src.WellName, src.Id);
            })
            .ForMember(d => d.SourceSample, o => o.MapFrom((src, _, _, ctx) =>
            {
                if (src.SourceSampleId is null) return null;
                if (!ctx.Items.TryGetValue("Session", out var sessionObj)
                    || sessionObj is not NHibernate.ISession session)
                {
                    _logger.LogError("Session not passed in Items when mapping WaterSample");
                    Debug.Assert(false, "Pass Session as NHibernate.ISession in Items when mapping WaterSample.");
                    return null;
                }
                return session.Get<WaterSampleEntity>(src.SourceSampleId.Value);
            }))
            .IgnoreAllMembers()
            // Для новых объектов Project/Id/SampledAt/WaterType/ClusterStationName/WellName заданы в конструкторе new CalcWaterSampleEntity(...) выше.
            // Для загруженных из БД эти поля уже актуальны — перезаписывать их не нужно.
            .ForMember(d => d.Chloride, o => o.MapFrom(s => s.Chloride))
            .ForMember(d => d.Carbonate, o => o.MapFrom(s => s.Carbonate))
            .ForMember(d => d.Bicarbonate, o => o.MapFrom(s => s.Bicarbonate))
            .ForMember(d => d.Sulfate, o => o.MapFrom(s => s.Sulfate))
            .ForMember(d => d.Calcium, o => o.MapFrom(s => s.Calcium))
            .ForMember(d => d.Magnesium, o => o.MapFrom(s => s.Magnesium))
            .ForMember(d => d.Sodium, o => o.MapFrom(s => s.Sodium))
            .ForMember(d => d.Equivalent, o => o.Ignore())
            .AfterMap((src, dest, ctx) =>
            {
                if (src.Equivalent is not null)
                {
                    if (dest.Equivalent is null)
                        dest.Equivalent = new CalcWaterSampleEquivalentRecord(dest);
                    var eq = dest.Equivalent;
                    dest.Equivalent = eq;
                    dest.Equivalent.Chloride = eq.Chloride;
                    dest.Equivalent.Carbonate = eq.Carbonate;
                    dest.Equivalent.Bicarbonate = eq.Bicarbonate;
                    dest.Equivalent.Sulfate = eq.Sulfate;
                    dest.Equivalent.Calcium = eq.Calcium;
                    dest.Equivalent.Magnesium = eq.Magnesium;
                    dest.Equivalent.Sodium = eq.Sodium;
                }
                else
                {
                    dest.Equivalent = null;
                }

                if (ctx.Items.TryGetValue("WaterSampleMap", out var mapObj)
                    && mapObj is Dictionary<CalcWaterSampleEntity, WaterSampleModel> map)
                {
                    map[dest] = src;
                }
            });

        _logger.LogDebug("Mapping: OilField → OilFieldEntity");
        CreateMap<OilFieldModel, OilFieldEntity>()
            .ConstructUsing((src, ctx) =>
            {
                if (!ctx.Items.TryGetValue("Session", out var sessionObj)
                    || sessionObj is not NHibernate.ISession session)
                {
                    _logger.LogError("Session not passed in Items when mapping OilField to OilFieldEntity");
                    Debug.Assert(false, "Pass Session as NHibernate.ISession in Items when mapping Project to CalcProjectEntity.");
                    return null!;
                }
                return session.Load<OilFieldEntity>(src.Id);
            })
            .ForAllMembers(o => o.Ignore());

        _logger.LogDebug("Mapping: DevTarget → DevTargetEntity");
        CreateMap<DevTargetModel, DevTargetEntity>()
            .ConstructUsing((src, ctx) =>
            {
                if (!ctx.Items.TryGetValue("Session", out var sessionObj)
                    || sessionObj is not NHibernate.ISession session)
                {
                    _logger.LogError("Session not passed in Items when mapping DevTarget to DevTargetEntity");
                    Debug.Assert(false, "Pass Session as NHibernate.ISession in Items when mapping Project to CalcProjectEntity.");
                    return null!;
                }
                return session.Load<DevTargetEntity>(src.Id);
            })
            .ForAllMembers(o => o.Ignore());

        _logger.LogDebug("Mapping: Project → CalcProjectEntity");
        CreateMap<ProjectModel, CalcProjectEntity>()
            .ConstructUsing((src, ctx) =>
            {
                CalcProjectEntity entity;
                if (src.Id is not null)
                {
                    _logger.LogDebug("Loading existing CalcProjectEntity by Id: {ProjectId}", src.Id);
                    if (!ctx.Items.TryGetValue("Session", out var sessionObj)
                        || sessionObj is not NHibernate.ISession session)
                    {
                        _logger.LogError("Session not passed in Items when mapping Project to CalcProjectEntity");
                        Debug.Assert(false, "Pass Session as NHibernate.ISession in Items when mapping Project to CalcProjectEntity.");
                        return null!;
                    }
                    entity = session.Get<CalcProjectEntity>(src.Id.Value);
                    entity.WaterSamples.Clear(); // перезаполнение
                }
                else
                {
                    _logger.LogDebug("Creating new CalcProjectEntity");
                    entity = new CalcProjectEntity(
                        ctx.Mapper.Map<OilFieldEntity>(src.OilField),
                        ctx.Mapper.Map<DevTargetEntity>(src.DevTarget),
                        src.Name,
                        src.Id);
                }
                ctx.Items["ParentProjectEntity"] = entity;
                return entity;
            })
            .IgnoreAllMembers()
            // Для новых объектов Id/OilField/DevTarget заданы в конструкторе new CalcProjectEntity(...) выше.
            // Для загруженных из БД эти поля уже актуальны — перезаписывать их не нужно.
            // WaterSamples - заполняется ниже
            .AfterMap((src, dest, ctx) =>
            {
                _logger.LogDebug("Mapping water samples for project Id: {Id}", src.Id);

                foreach (var ws in src.WaterSamples.Select(ws => ctx.Mapper.Map<CalcWaterSampleEntity>(ws)))
                    dest.WaterSamples.Add(ws);
            });

        _logger.LogInformation("ProjectProfile configured successfully");
    }
}
