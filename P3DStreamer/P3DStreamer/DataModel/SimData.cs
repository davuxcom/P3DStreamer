using System;
using System.Runtime.InteropServices;

namespace P3DStreamer.DataModel
{
    /*
    public enum AvionicsKind
    {
        G1000,
        G3000,
        G3X,
    }

    public enum EngineKind
    {
        Piston,
        Turbine,
        Jet
    }
     */
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GENERIC_DATA
    {
        [SimLink(Name = "Title", Type = null)]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string title;
        [SimLink(Name = "Sim on Ground", Type = "Bool")]
        public double sim_on_ground;
        [SimLink(Name = "Ground Velocity", Type = "Knots")]
        public double groundspeed_kts;
        [SimLink(Name = "FUEL TOTAL QUANTITY WEIGHT", Type = "Pounds")]
        public double fuel_total_lbs;
        [SimLink(Name = "ENG FUEL FLOW PPH:1", Type = "Pounds per hour")]
        public double eng1_fuel_flow_pph;
        [SimLink(Name = "ENG FUEL FLOW PPH:2", Type = "Pounds per hour")]
        public double eng2_fuel_flow_pph;
        [SimLink(Name = "FUEL TANK SELECTOR:1", Type = "Enum")]
        public double fuel_tank_selector_1;
        [SimLink(Name = "STRUCTURAL ICE PCT", Type = "Percent")]
        public double structural_ice_pct;
        [SimLink(Name = "BRAKE PARKING POSITION", Type = "Bool")]
        public double parking_brake;
        [SimLink(Name = "GEAR TOTAL PCT EXTENDED", Type = "Percent")]
        public double gear_ext_pct;
        [SimLink(Name = "FLAPS HANDLE INDEX", Type = "Number")]
        public double flaps_position_idx;
        [SimLink(Name = "FUEL LEFT QUANTITY", Type = "Number")]
        public double fuel_left_qty;
        [SimLink(Name = "FUEL RIGHT QUANTITY", Type = "Number")]
        public double fuel_right_qty;
        [SimLink(Name = "RADIO HEIGHT", Type = "Feet")]
        public double radio_Height;
        [SimLink(Name = "AUTOPILOT ALTITUDE LOCK VAR", Type = "Number")]
        public double ap_desired_alt;
        [SimLink(Name = "Indicated Altitude", Type = "Feet")]
        public double indicated_alt;
        [SimLink(Name = "ELEVATOR TRIM INDICATOR", Type = "Number")]
        public double elev_trim;
        [SimLink(Name = "PITOT HEAT", Type = "Bool")]
        public double pitot_heat_enabled;
        [SimLink(Name = "AMBIENT TEMPERATURE", Type = "Celsius")]
        public double outside_air_temp_c;
        [SimLink(Name = "AMBIENT IN CLOUD", Type = "Bool")]
        public double is_in_cloud;
        [SimLink(Name = "GENERAL ENG THROTTLE LEVER POSITION:1", Type = "Percent")]
        public double eng1_throttle_pct;
        [SimLink(Name = "GENERAL ENG RPM:1", Type = "Rpm")]
        public double eng1_rpm;
        [SimLink(Name = "GENERAL ENG RPM:2", Type = "Rpm")]
        public double eng2_rpm;

        [SimLink(Name = "ELECTRICAL BATTERY LOAD:1", Type = "Amperes")]
        public double elec_batt_load_1;
        [SimLink(Name = "ELECTRICAL BATTERY LOAD:2", Type = "Amperes")]
        public double elec_batt_load_2;
        [SimLink(Name = "ELECTRICAL BATTERY LOAD:3", Type = "Amperes")]
        public double elec_batt_load_3;
        [SimLink(Name = "ELECTRICAL BATTERY VOLTAGE:1", Type = "Amperes")]
        public double elec_batt1_volts;
        [SimLink(Name = "ELECTRICAL BATTERY VOLTAGE:2", Type = "Amperes")]
        public double elec_batt2_volts;
        [SimLink(Name = "ELECTRICAL BATTERY VOLTAGE:3", Type = "Amperes")]
        public double elec_batt3_volts;

        [SimLink(Name = "GENERAL ENG FUEL PUMP ON:1", Type = "Bool")]
        public double eng1_fuel_pump_on;
        [SimLink(Name = "GENERAL ENG DAMAGE PERCENT:1", Type = "Percent")]
        public double eng1_damage_pct;
        [SimLink(Name = "SUCTION PRESSURE", Type = "inHg")]
        public double suction_pressure;
        [SimLink(Name = "GPS WP DISTANCE", Type = "Meters")]
        public double gps_wp_distance;
        [SimLink(Name = "GPS WP ETE", Type = "Seconds")]
        public double gps_WP_ETE_seconds;
        [SimLink(Name = "GPS ETE", Type = "Seconds")]
        public double gps_ETE_seconds;
        [SimLink(Name = "GPS WP CROSS TRK", Type = "Meters")]
        public double gps_wp_xtrack_meters;
        [SimLink(Name = "AUTOPILOT MASTER", Type = "Bool")]
        public double ap_master_on;
        [SimLink(Name = "FUEL TOTAL CAPACITY", Type = "Gallons")]
        public double fuel_total_capacity_gallons;
        [SimLink(Name = "FUEL WEIGHT PER GALLON", Type = "Pounds")]
        public double fuel_pounds_per_1gallon;
        [SimLink(Name = "IS GEAR RETRACTABLE", Type = "Bool")]
        public double gear_is_retractable;
        [SimLink(Name = "AUTOPILOT MASTER", Type = "Bool")]
        public double ap_master;
        [SimLink(Name = "AUTOPILOT FLIGHT DIRECTOR ACTIVE", Type = "Bool")]
        public double ap_fd_active;
        [SimLink(Name = "AUTOPILOT HEADING LOCK", Type = "Bool")]
        public double ap_mode_HDG;
        [SimLink(Name = "AUTOPILOT WING LEVELER", Type = "Bool")]
        public double ap_mode_lvl;

        


        [SimLink(Name = "AUTOPILOT ALTITUDE LOCK", Type = "Bool")]
        public double ap_mode_ALT;
        [SimLink(Name = "AUTOPILOT VERTICAL HOLD", Type = "Bool")]
        public double ap_mode_VS;
        [SimLink(Name = "AUTOPILOT APPROACH HOLD", Type = "Bool")]
        public double ap_mode_APPR;
        [SimLink(Name = "AUTOPILOT NAV1 LOCK", Type = "Bool")]
        public double ap_mode_NAV;
        [SimLink(Name = "AUTOPILOT FLIGHT LEVEL CHANGE", Type = "Bool")]
        public double ap_mode_flc;
        [SimLink(Name = "AUTOPILOT AIRSPEED HOLD", Type = "Bool")]
        public double ap_ias_hold;
        [SimLink(Name = "AUTOPILOT NAV SELECTED", Type = "Number")]
        public double ap_nav_index;
        [SimLink(Name = "AUTOPILOT THROTTLE ARM", Type = "Bool")]
        public double ap_at_is_armed;
        [SimLink(Name = "AUTOTHROTTLE ACTIVE", Type = "Bool")]
        public double ap_at_is_active;

        [SimLink(Name = "AIRSPEED TRUE", Type = "Knots")]
        public double airspeed_tas_kts;
        [SimLink(Name = "AIRSPEED INDICATED", Type = "Knots")]
        public double airspeed_ias_kts;
        [SimLink(Name = "AIRSPEED BARBER POLE", Type = "Knots")]
        public double airspeed_never_exceed_kts;

        [SimLink(Name = "MAX RATED ENGINE RPM", Type = "Rpm")]
        public double eng_rpm_max;
        [SimLink(Name = "ENGINE MIXURE AVAILABLE", Type = "Bool")]
        public double eng_mix_is_available;
        [SimLink(Name = "GENERAL ENG MIXTURE LEVER POSITION:1", Type = "Percent")]
        public double eng1_mix_percent;
        [SimLink(Name = "GENERAL ENG PROPELLER LEVER POSITION:1", Type = "Percent")]
        public double eng1_prop_percent;
        [SimLink(Name = "AUTOPILOT AIRSPEED HOLD VAR", Type = "Knots")]
        public double ap_desired_airspeed;
        [SimLink(Name = "SPOILER AVAILABLE", Type = "Bool")]
        public double is_spoilers_available;
        [SimLink(Name = "SPOILERS HANDLE POSITION", Type = "Percent")]
        public double spoilers_position;

        [SimLink(Name = "LIGHT LANDING ON", Type = "Bool")]
        public double light_landing_on;
        [SimLink(Name = "LIGHT TAXI", Type = "Bool")]
        public double light_taxi_on;
        [SimLink(Name = "LIGHT BEACON", Type = "Bool")]
        public double light_beacon_on;
        [SimLink(Name = "LIGHT NAV", Type = "Bool")]
        public double light_nav_on;
        [SimLink(Name = "LIGHT LOGO", Type = "Bool")]
        public double light_logo_on;
        [SimLink(Name = "LIGHT WING", Type = "Bool")]
        public double light_wing_on;
        [SimLink(Name = "LIGHT RECOGNITION", Type = "Bool")]
        public double light_recog_on;
        [SimLink(Name = "LIGHT STROBE", Type = "Bool")]
        public double light_strobe_on;
        [SimLink(Name = "LIGHT PANEL", Type = "Bool")]
        public double light_panel_on;
        [SimLink(Name = "LIGHT CABIN", Type = "Bool")]
        public double light_cabin_on;
        [SimLink(Name = "STRUCTURAL DEICE SWITCH", Type = "Bool")]
        public double deice_structural_on;
        [SimLink(Name = "GENERAL ENG ANTI ICE POSITION:1", Type = "Bool")]
        public double eng1_antiice_on;


        [SimLink(Name = "APU PCT RPM", Type = "Percent over 100")]
        public double apu_pct_rpm;
        [SimLink(Name = "G FORCE", Type = "GForce")]
        public double gforce;
        [SimLink(Name = "GENERAL ENG MASTER ALTERNATOR:1", Type = "Bool")]
        public double eng1_gen_on;
        [SimLink(Name = "GENERAL ENG MASTER ALTERNATOR:2", Type = "Bool")]
        public double eng2_gen_on;

        [SimLink(Name = "APU GENERATOR SWITCH", Type = "Bool")]
        public double apu_gen_on;
        [SimLink(Name = "APU VOLTS", Type = "Volts")]
        public double apu_gen_volts;
        [SimLink(Name = "ELECTRICAL MASTER BATTERY:1", Type = "Bool")]
        public double elec_batt1_on;
        [SimLink(Name = "ELECTRICAL MASTER BATTERY:2", Type = "Bool")]
        public double elec_batt2_on;
        [SimLink(Name = "ELECTRICAL MASTER BATTERY:3", Type = "Bool")]
        public double elec_batt3_on;
        [SimLink(Name = "EXTERNAL POWER AVAILABLE", Type = "Bool")]
        public double elec_ext_pwr_available;
        [SimLink(Name = "EXTERNAL POWER ON", Type = "Bool")]
        public double elec_ext_pwr_on;
        [SimLink(Name = "ELECTRICAL TOTAL LOAD AMPS", Type = "Amperes")]
        public double elec_total_load;

     
        [SimLink(Name = "ENG HYDRAULIC PRESSURE:1", Type = "pound-force per square inch")]
        public double eng1_hydraulic_press;
        [SimLink(Name = "ENG HYDRAULIC PRESSURE:2", Type = "pound-force per square inch")]
        public double eng2_hydraulic_press;

        [SimLink(Name = "GENERAL ENG STARTER:1", Type = "Bool")]
        public double eng1_starter_on;
        [SimLink(Name = "GENERAL ENG STARTER:2", Type = "Bool")]
        public double eng2_starter_on;
        [SimLink(Name = "TURB ENG IGNITION SWITCH:1", Type = "Bool")]
        public double eng1_on;
        [SimLink(Name = "TURB ENG IGNITION SWITCH:2", Type = "Bool")]
        public double eng2_on;

        [SimLink(Name = "ENG OIL PRESSURE:1", Type = "pound-force per square inch")]
        public double eng1_oil_press;
        [SimLink(Name = "ENG OIL PRESSURE:2", Type = "pound-force per square inch")]
        public double eng2_oil_press;



        [SimLink(Name = "GENERAL ENG FUEL VALVE:1", Type = "Bool")]
        public double eng1_fuel_valve_on;
        [SimLink(Name = "GENERAL ENG FUEL VALVE:2", Type = "Bool")]
        public double eng2_fuel_valve_on;
        [SimLink(Name = "SPOILERS ARMED", Type = "Bool")]
        public double spoilers_is_armed;

        [SimLink(Name = "ELECTRICAL GENALT BUS VOLTAGE:1", Type = "Volts")]
        public double eng1_gen_volts;
        [SimLink(Name = "ELECTRICAL GENALT BUS VOLTAGE:2", Type = "Volts")]
        public double eng2_gen_volts;

        [SimLink(Name = "CIRCUIT ON:31", Type = "Bool")]
        public double elec_circuit_31_on;
        

        /*
        [SimLink(Name = "BUS CONNECTION ON:1", Type = "Bool")]
        public double elec_bus_1_on;
        [SimLink(Name = "BUS CONNECTION ON:2", Type = "Bool")]
        public double elec_bus_2_on;
        [SimLink(Name = "BUS CONNECTION ON:3", Type = "Bool")]
        public double elec_bus_3_on;
        [SimLink(Name = "BUS CONNECTION ON:4", Type = "Bool")]
        public double elec_bus_4_on;
        [SimLink(Name = "BUS CONNECTION ON:5", Type = "Bool")]
        public double elec_bus_5_on;
        [SimLink(Name = "BUS CONNECTION ON:6", Type = "Bool")]
        public double elec_bus_6_on;
        [SimLink(Name = "BUS CONNECTION ON:7", Type = "Bool")]
        public double elec_bus_7_on;
        [SimLink(Name = "BUS CONNECTION ON:8", Type = "Bool")]
        public double elec_bus_8_on;
        */
    }

    public static class GENERIC_DATA_Extensions
    {
        public static TimeSpan CalculateEndurance(this GENERIC_DATA newData)
        {
            // VL-3: 12lbs
            // C172+G1k: 20lbs
            var unusable_fuel_lbs = 18;
            var fuel_qty_lbs = newData.fuel_total_lbs;
            var endurance = (fuel_qty_lbs - unusable_fuel_lbs) / (newData.eng1_fuel_flow_pph + newData.eng2_fuel_flow_pph);

            if (newData.eng1_fuel_flow_pph == 0)
            {
                endurance = 1000;
            }
            return TimeSpan.FromHours(endurance);
        }
    }
}
