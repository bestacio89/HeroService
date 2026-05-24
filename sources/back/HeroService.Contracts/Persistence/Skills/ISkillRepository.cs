using HeroService.Domain.Heroes.Skills;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.Persistence.Skills;

public interface ISkillRepository :
    INameLookupRepository<Skill, Guid>
{

}