using System.Collections.Generic;
using Database;
using ExpandedDuplicantMultitools.Equipment;
using LibNoiseDotNet.Graphics.Tools.Noise.Combiner;

namespace ExpandedDuplicantMultitools.Skills
{
    public static class ExtraSkills
    {
        private static Skill NeutroniumDigging;
        public static SkillPerk NeutroniumDiggingPerk;

        public const string NeutroniumDiggingSkillId = "asquared31415_" + nameof(NeutroniumDigging);
        public const string NeutroniumDiggingPerkId = "asquared31415_" + nameof(NeutroniumDiggingPerk);

        private static SkillPerks _skillPerks;
        private static Database.Skills _skills;

        private static Skill CreateSkill(Skill skill, List<string> priorSkillIds, List<SkillPerk> skillPerks)
        {
            var s = _skills.Add(skill);
            s.priorSkills = priorSkillIds;
            s.perks = skillPerks;
            return s;
        }

        public static void InitializeSkillsAndPerks()
        {
            _skillPerks = Db.Get().SkillPerks;
            _skills = Db.Get().Skills;

            NeutroniumDiggingPerk = _skillPerks.Add(
                new SimpleSkillPerk(
                    NeutroniumDiggingPerkId,
                    MULTITOOLSSTRINGS.SKILLPERKS.NEUTRONIUM_DIGGING.DESCRIPTION
                )
            );

            NeutroniumDigging = CreateSkill(
                new ConditionalSkill(
                    NeutroniumDiggingSkillId,
                    MULTITOOLSSTRINGS.SKILLS.NEUTRONIUM_DIGGING.NAME,
                    MULTITOOLSSTRINGS.SKILLS.NEUTRONIUM_DIGGING.DESCRIPTION,
                    3,
                    "hat_role_mining3",
                    "skillbadge_role_mining3",
                    Db.Get().SkillGroups.Mining.Id,
                    NeutroniumPerk.GivesPerk,
                    NeutroniumPerk.GivesId
                ),
                new List<string> {_skills.Mining3.Id},
                new List<SkillPerk> {NeutroniumDiggingPerk}
            );
        }
    }

    public static class NeutroniumPerk
    {
        private static bool GivesPerkBase(MinionResume resume)
        {
            if (!(resume.GetComponent<MinionIdentity>().GetEquipment()
                .GetSlot(ExpandedAssignableSlots.ToolAttachment)
                .assignable is Equippable equippedTool)) return false;

            var attachment = equippedTool.GetComponent<MultitoolAttachment>();
            if (attachment == null) return false;

            return attachment.toolType == MultitoolAttachment.AttachmentType.NeutroniumMiner;
        }

        public static bool GivesPerk(MinionResume resume, SkillPerk perk)
        {
            return perk == ExtraSkills.NeutroniumDiggingPerk && GivesPerkBase(resume);
        }

        public static bool GivesId(MinionResume resume, HashedString perkId)
        {
            return perkId == ExtraSkills.NeutroniumDiggingPerkId && GivesPerkBase(resume);
        }
    }
}