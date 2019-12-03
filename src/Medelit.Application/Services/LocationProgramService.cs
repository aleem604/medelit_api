using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TinCore.Application.Interfaces;
using TinCore.Application.ViewModels;
using TinCore.Common;
using TinCore.Common.Enums;
using TinCore.Domain.Commands;
using TinCore.Domain.Core.Bus;
using TinCore.Domain.Interfaces;

namespace TinCore.Application.Services
{
    public class LocationProgramService : ILocationProgramService
    {
        private readonly IMapper _mapper;
        private readonly ILocationProgramRepository _locationProgramRepository;
        private readonly IConfiguration _configuration;
        private readonly IMediatorHandler Bus;

        public LocationProgramService(IMapper mapper,
                                  ILocationProgramRepository locationProgramRepository,
                                  IConfiguration configuration,
                                  IMediatorHandler bus)
        {
            _mapper = mapper;
            _locationProgramRepository = locationProgramRepository;
            _configuration = configuration;
            Bus = bus;
        }


        public dynamic GetLocationPrograms(string version, long id)
        {
            return _locationProgramRepository.GetAll()
                .Where(x => x.LocationId == id)
                    .Select(
                    (o) => new
                    {
                        id = o.Id,
                        name = o.Name,
                        desc1 = Utils.ProcessXML(o.MetaData, "desc1"),
                        desc2 = Utils.ProcessXML(o.MetaData, "desc2"),
                        url = Utils.ProcessXML(o.MetaData, "url"),
                        image_url = Utils.ProcessXML(o.MetaData, "image_url")
                    }
                )
                .OrderBy(o => o.id)
                .ToList();
        }



        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}
