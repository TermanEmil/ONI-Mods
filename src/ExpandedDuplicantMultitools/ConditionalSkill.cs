using System;
using Database;

namespace ExpandedDuplicantMultitools
{
    public class ConditionalSkill : Skill
    {
        private readonly Func<MinionResume, SkillPerk, bool>    _perkCondition;
        private readonly Func<MinionResume, HashedString, bool> _stringCondition;

        public ConditionalSkill(
            string id,
            string name,
            string description,
            int tier,
            string hat,
            string badge,
            string skillGroup,
            Func<MinionResume, SkillPerk, bool> perkCondition = null,
            Func<MinionResume, HashedString, bool> stringCondition = null
        ) : base( id, name, description, tier, hat, badge, skillGroup )
        {
            _perkCondition = perkCondition;
            _stringCondition = stringCondition;
        }

        public bool GivesPerk( MinionResume resume, SkillPerk perk )
        {
            return _perkCondition != null && _perkCondition( resume, perk );
        }

        public bool GivesPerk( MinionResume resume, HashedString perk )
        {
            return _stringCondition != null && _stringCondition( resume, perk );
        }
    }
}
