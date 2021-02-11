﻿namespace P3DStreamer.DataModel
{
    public enum SimEvents
    {
        PITOT_HEAT_ON,
        PITOT_HEAT_TOGGLE,
        FUEL_SELECTOR_ALL,
        AP_MASTER,
        AP_ALT_VAR_SET_ENGLISH,
        AP_VS_VAR_SET_ENGLISH,
        AP_APR_HOLD_ON,
        AP_LOC_HOLD_ON,
        AP_HDG_HOLD_ON,
        AP_WING_LEVELER_ON,
        FLIGHT_LEVEL_CHANGE,
        AP_ALT_HOLD_ON,
        AP_NAV1_HOLD_ON,
        AP_VS_VAR_INC,
        AP_VS_VAR_DEC,
        AP_VS_HOLD,
        AP_SPD_VAR_INC,
        AP_SPD_VAR_DEC,
        AP_ALT_VAR_INC,
        AP_ALT_VAR_DEC,
        AUTO_THROTTLE_ARM,
        TOGGLE_FLIGHT_DIRECTOR,
        ALTITUDE_BUG_SELECT,
        MIXTURE_SET,
        MIXTURE1_RICH,
        MIXTURE2_RICH,
        MIXTURE1_LEAN,
        MIXTURE2_LEAN,
        PROP_PITCH_SET,
        THROTTLE_SET,

        PANEL_LIGHTS_TOGGLE,
        LANDING_LIGHTS_TOGGLE,
        STROBES_TOGGLE,
        TOGGLE_BEACON_LIGHTS,
        TOGGLE_TAXI_LIGHTS,
        TOGGLE_LOGO_LIGHTS,
        TOGGLE_RECOGNITION_LIGHTS,
        TOGGLE_WING_LIGHTS,
        TOGGLE_NAV_LIGHTS,
        TOGGLE_CABIN_LIGHTS,

        TOGGLE_STRUCTURAL_DEICE,
        ANTI_ICE_TOGGLE,
        KOHLSMAN_INC,
        KOHLSMAN_DEC,
        BAROMETRIC,
        APU_STARTER,
        APU_OFF_SWITCH,
        ELECTRICAL_BUS_TO_BUS_CONNECTION_TOGGLE,
        ELECTRICAL_CIRCUIT_TOGGLE,
        TOGGLE_EXTERNAL_POWER,
        TOGGLE_ALTERNATOR1,
        TOGGLE_ALTERNATOR2,
        APU_GENERATOR_SWITCH_TOGGLE,
        TOGGLE_MASTER_BATTERY,
        TOGGLE_STARTER1,
        TOGGLE_STARTER2,
        TURBINE_IGNITION_SWITCH_SET1,
        TURBINE_IGNITION_SWITCH_SET2,
        TOGGLE_FUEL_VALVE_ENG1,
        TOGGLE_FUEL_VALVE_ENG2,
        SPOILERS_ARM_TOGGLE,

      //  MOBIFLIGHT_AS1000_MFD_RANGE_INC,
      //  MOBIFLIGHT_AS1000_MFD_RANGE_DEC,
      //  MOBIFLIGHT_AS3000_MFD_RNG_DEZOOM,
       // MOBIFLIGHT_AS3000_MFD_RNG_Zoom,
    }
}
