using HeroService.Domain.Heroes.Skills;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.Persistence;

public interface ISkillRepository :
    INameLookupRepository<Skill, Guid>
{

}