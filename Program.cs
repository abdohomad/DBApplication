using System;
using Npgsql;

class Program
{
    static void Main()
    {
       
        string connString = "Host=localhost;Username=postgres;Password=root;Database=Pharmacy Prescription Management System";

        using (var conn = new NpgsqlConnection(connString))
        {
            conn.Open();

            while (true)
            {
                Console.WriteLine("Select an option:");
                Console.WriteLine("1. List prescriptions for a patient in September 2023");
                Console.WriteLine("2. List drugs sold by a pharmacy");
                Console.WriteLine("3. List drugs sold by a pharmaceutical company");
                Console.WriteLine("4. List all pharmacies in descending order by name");
                Console.WriteLine("5. List doctors with more than 2 patients and total patient count");
                Console.WriteLine("6. List contracts for a pharmacy with supervisor information");
                Console.WriteLine("7. List total number of contracts for each pharmaceutical company");
                Console.WriteLine("8. Insert a new drug sold by a pharmaceutical company");
                Console.WriteLine("9. Delete a pharmaceutical company and associated drugs");
                Console.WriteLine("10. Update the address information for a pharmacy");
                Console.WriteLine("0. Exit");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter patient SSN: ");
                        string patient_ssn = Console.ReadLine();
                        Query1(patient_ssn, conn);
                        break;
                    case "2":
                        Console.Write("Enter pharmacy name: ");
                        string pharmacy_name = Console.ReadLine();
                        Query2(pharmacy_name, conn);
                        break;
                    case "3":
                        Console.Write("Enter pharmaceutical company name: ");
                        string pharmaceutical_company_name = Console.ReadLine();
                        Query3(pharmaceutical_company_name, conn);
                        break;
                    case "4":
                        Query4(conn);
                        break;
                    case "5":
                        Query5(conn);
                        break;
                    case "6":
                        Console.Write("Enter pharmacy name: ");
                        string pharmacyName = Console.ReadLine();
                        Query6(pharmacyName, conn);
                        break;
                    case "7":
                        Query7(conn);
                        break;
                    case "8":
                        Console.Write("Enter drug trade name: ");
                        string drugTradeName = Console.ReadLine();
                        Console.Write("Enter formula: ");
                        string formula = Console.ReadLine();
                        Console.Write("Enter pharmaceutical company name: ");
                        string companyName = Console.ReadLine();
                        Query8(drugTradeName, formula, companyName, conn);
                        Console.WriteLine("Drug inserted successfully.");
                        break;
                    case "9":
                        Console.Write("Enter pharmaceutical company name to delete: ");
                        string companyToDelete = Console.ReadLine();
                        Query9(companyToDelete, conn);
                        Console.WriteLine("Pharmaceutical company deleted successfully.");
                        break;
                    case "10":
                        Console.Write("Enter pharmacy name: ");
                        string pharmacyNameToUpdate = Console.ReadLine();
                        Console.Write("Enter new address: ");
                        string newAddress = Console.ReadLine();
                        Query10(pharmacyNameToUpdate, newAddress, conn);
                        Console.WriteLine("Pharmacy address updated successfully.");
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please enter a valid option.");
                        break;
                }
            }
        }
    }

    static void Query1(string patient_ssn, NpgsqlConnection conn)
    {
        using (var cmd = new NpgsqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"
                SELECT p.latest_date, p.quantity, d.drug_trade_name, d.formula, pc.Pharmaceutical_company_name,
                       doc.person_ssn, doc.first_name, doc.last_name, doc.speciality, doc.experience_years
                FROM Prescription p
                JOIN Drug d ON p.drug_trade_name = d.drug_trade_name
                JOIN Doctor doc ON p.doctor_ssn = doc.person_ssn
                JOIN Pharmaceutical_company pc ON d.Pharmaceutical_company_name = pc.Pharmaceutical_company_name
                WHERE p.patient_ssn = @patient_ssn AND EXTRACT(MONTH FROM p.latest_date) = 9";
            cmd.Parameters.AddWithValue("patient_ssn", patient_ssn);

            using (var reader = cmd.ExecuteReader())
            {
                Console.WriteLine("----------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("| Latest Date | Quantity | Drug Trade Name | Formula | Pharmaceutical Company | Doctor SSN | First Name | Last Name | Speciality | Experience Years |");
                Console.WriteLine("----------------------------------------------------------------------------------------------------------------");

                while (reader.Read())
                {
                    Console.WriteLine($"| {reader["latest_date"],-12} | {reader["quantity"],-8} | {reader["drug_trade_name"],-16} | {reader["formula"],-7} | {reader["Pharmaceutical_company_name"],-23} | {reader["person_ssn"],-9} | {reader["first_name"],-11} | {reader["last_name"],-10} | {reader["speciality"],-11} | {reader["experience_years"],-16} |");
                }

                Console.WriteLine("----------------------------------------------------------------------------------------------------------------");
            }
        }
    }

    static void Query2(string pharmacy_name, NpgsqlConnection conn)
    {
        using (var cmd = new NpgsqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"
            SELECT d.*
            FROM Drug d
            JOIN Sale s ON d.drug_trade_name = s.drug_trade_name
            WHERE s.pharmacy_name = @pharmacy_name";
            cmd.Parameters.AddWithValue("pharmacy_name", pharmacy_name);

            using (var reader = cmd.ExecuteReader())
            {
                Console.WriteLine("--------------------------------------------------------------------------------------------");
                Console.WriteLine("| Drug Trade Name | Formula | Pharmaceutical Company | Price | Quantity | Expiry Date |");
                Console.WriteLine("--------------------------------------------------------------------------------------------");

                while (reader.Read())
                {
                    Console.WriteLine($"| {reader["drug_trade_name"],-16} | {reader["formula"],-7} | {reader["Pharmaceutical_company_name"],-23} | {reader["price"],-6} | {reader["quantity"],-8} | {reader["expiry_date"],-12} |");
                }

                Console.WriteLine("--------------------------------------------------------------------------------------------");
            }
        }
    }


    static void Query3(string pharmaceutical_company_name, NpgsqlConnection conn)
    {
        using (var cmd = new NpgsqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"
                SELECT d.*
                FROM Drug d
                WHERE d.Pharmaceutical_company_name = @pharmaceutical_company_name";
            cmd.Parameters.AddWithValue("pharmaceutical_company_name", pharmaceutical_company_name);

            using (var reader = cmd.ExecuteReader())
            {
                Console.WriteLine("--------------------------------------------------------------------------------------------");
                Console.WriteLine("| Drug Trade Name | Formula | Price | Quantity | Expiry Date |");
                Console.WriteLine("--------------------------------------------------------------------------------------------");

                while (reader.Read())
                {
                    Console.WriteLine($"| {reader["drug_trade_name"],-16} | {reader["formula"],-7} | {reader["price"],-6} | {reader["quantity"],-8} | {reader["expiry_date"],-12} |");
                }

                Console.WriteLine("--------------------------------------------------------------------------------------------");
            }
        }
    }

    static void Query4(NpgsqlConnection conn)
    {
        using (var cmd = new NpgsqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"
            SELECT *
            FROM Pharmacy
            ORDER BY pharmacy_name DESC";

            using (var reader = cmd.ExecuteReader())
            {
                Console.WriteLine("-----------------------------------------------------------");
                Console.WriteLine("| Pharmacy Name     | Address         | Phone Number     |");
                Console.WriteLine("-----------------------------------------------------------");

                while (reader.Read())
                {
                    Console.WriteLine($"| {reader["pharmacy_name"],-18} | {reader["address"],-15} | {reader["phone"],-16} |");
                }

                Console.WriteLine("-----------------------------------------------------------");
            }
        }
    }


    static void Query5(NpgsqlConnection conn)
    {
        using (var cmd = new NpgsqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"
            SELECT doc.person_ssn, COUNT(pat.person_ssn) as patient_count
            FROM Doctor doc
            LEFT JOIN Patient pat ON doc.person_ssn = pat.doctor_ssn
            GROUP BY doc.person_ssn
            HAVING COUNT(pat.person_ssn) > 2
            ORDER BY doc.person_ssn";

            using (var reader = cmd.ExecuteReader())
            {
                Console.WriteLine("-----------------------------------------------------------");
                Console.WriteLine("| Doctor SSN        | Patient Count   |");
                Console.WriteLine("-----------------------------------------------------------");

                while (reader.Read())
                {
                    Console.WriteLine($"| {reader["person_ssn"],-18} | {reader["patient_count"],-15} |");
                }

                Console.WriteLine("-----------------------------------------------------------");
            }
        }
    }


    static void Query6(string pharmacyName, NpgsqlConnection conn)
    {
        using (var cmd = new NpgsqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"
            SELECT
                pc.Pharmaceutical_company_name,
                pc.phone_number,
                c.start_date::DATE as start_date,
                c.end_date::DATE as end_date,
                s.supervisor_id,
                s.first_name,
                s.last_name
            FROM
                Contract c
            JOIN
                Supervisor s ON c.supervisor_id = s.supervisor_id
            JOIN
                Pharmaceutical_company pc ON c.Pharmaceutical_company_name = pc.Pharmaceutical_company_name
            WHERE
                c.pharmacy_name = @pharmacyName";
            cmd.Parameters.AddWithValue("pharmacyName", pharmacyName);

            using (var reader = cmd.ExecuteReader())
            {
                Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("| Pharmaceutical Company | Phone Number  | Start Date          | End Date       | Supervisor ID      | First Name  | Last Name |");
                Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------");

                while (reader.Read())
                {
                    Console.WriteLine($"| {reader["Pharmaceutical_company_name"],-22} | {reader["phone_number"],-13} | {reader["start_date"],-12} | {reader["end_date"],-18} | {reader["supervisor_id"],-14} | {reader["first_name"],-12} | {reader["last_name"],-9} |");
                }

                Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------");
            }
        }
    }



    static void Query7(NpgsqlConnection conn)
    {
        using (var cmd = new NpgsqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"
            SELECT pc.Pharmaceutical_company_name, COUNT(c.*) as contract_count
            FROM Pharmaceutical_company pc
            LEFT JOIN Contract c ON pc.Pharmaceutical_company_name = c.Pharmaceutical_company_name
            GROUP BY pc.Pharmaceutical_company_name
            ORDER BY pc.Pharmaceutical_company_name";

            using (var reader = cmd.ExecuteReader())
            {
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("| Pharmaceutical Company | Contract Count |");
                Console.WriteLine("--------------------------------------------------");

                while (reader.Read())
                {
                    Console.WriteLine($"| {reader["Pharmaceutical_company_name"],-24} | {reader["contract_count"],-15} |");
                }

                Console.WriteLine("--------------------------------------------------");
            }
        }
    }


    static void Query8(string drugTradeName, string formula, string companyName, NpgsqlConnection conn)
    {
        using (var cmd = new NpgsqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = @"
            INSERT INTO Drug (drug_trade_name, formula, Pharmaceutical_company_name)
            VALUES (@drugTradeName, @formula, @companyName)
            RETURNING *";

            cmd.Parameters.AddWithValue("drugTradeName", drugTradeName);
            cmd.Parameters.AddWithValue("formula", formula);
            cmd.Parameters.AddWithValue("companyName", companyName);

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    Console.WriteLine("--------------------------------------------------------------------------------------------");
                    Console.WriteLine("| Drug Trade Name | Formula | Pharmaceutical Company |");
                    Console.WriteLine("--------------------------------------------------------------------------------------------");
                    Console.WriteLine($"| {reader["drug_trade_name"],-16} | {reader["formula"],-7} | {reader["Pharmaceutical_company_name"],-23} |");
                    Console.WriteLine("--------------------------------------------------------------------------------------------");
                    Console.WriteLine("Drug inserted successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to insert the drug.");
                }
            }
        }
    }


    static void Query9(string companyToDelete, NpgsqlConnection conn)
    {
        using (var cmd = new NpgsqlCommand())
        {
            cmd.Connection = conn;

            // Fetch the data of the records to be deleted before deletion
            cmd.CommandText = @"
            SELECT *
            FROM Pharmaceutical_company
            WHERE Pharmaceutical_company_name = @companyToDelete";

            cmd.Parameters.AddWithValue("companyToDelete", companyToDelete);

            using (var reader = cmd.ExecuteReader())
            {
                Console.WriteLine("------------------------------------------------------------------");
                Console.WriteLine("| Pharmaceutical Company | phone |");
                Console.WriteLine("------------------------------------------------------------------");

                while (reader.Read())
                {
                    Console.WriteLine($"| {reader["Pharmaceutical_company_name"],-24} | {reader["phone"],-37} |");
                }

                Console.WriteLine("------------------------------------------------------------------");
            }

            // Delete the pharmaceutical company and its associated drugs
            cmd.CommandText = @"
            DELETE FROM Pharmaceutical_company
            WHERE Pharmaceutical_company_name = @companyToDelete;
            
            DELETE FROM Drug
            WHERE Pharmaceutical_company_name = @companyToDelete";

            cmd.ExecuteNonQuery();

            Console.WriteLine("Pharmaceutical company and associated drugs deleted successfully.");
        }
    }


    static void Query10(string pharmacyNameToUpdate, string newAddress, NpgsqlConnection conn)
    {
        using (var cmd = new NpgsqlCommand())
        {
            cmd.Connection = conn;

            // Fetch the data of the record before update
            cmd.CommandText = @"
            SELECT *
            FROM Pharmacy
            WHERE pharmacy_name = @pharmacyNameToUpdate";

            cmd.Parameters.AddWithValue("pharmacyNameToUpdate", pharmacyNameToUpdate);

            using (var reader = cmd.ExecuteReader())
            {
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("| Pharmacy Name     | Address         | Phone Number     |");
                Console.WriteLine("--------------------------------------------------");

                while (reader.Read())
                {
                    Console.WriteLine($"| {reader["pharmacy_name"],-18} | {reader["address"],-15} | {reader["phone_number"],-16} |");
                }

                Console.WriteLine("--------------------------------------------------");
            }

            // Perform the update
            cmd.CommandText = @"
            UPDATE Pharmacy
            SET address = @newAddress
            WHERE pharmacy_name = @pharmacyNameToUpdate";

            cmd.ExecuteNonQuery();

            // Fetch the data of the record after update
            cmd.CommandText = @"
            SELECT *
            FROM Pharmacy
            WHERE pharmacy_name = @pharmacyNameToUpdate";

            using (var reader = cmd.ExecuteReader())
            {
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("| Pharmacy Name     | Address         | Phone Number     |");
                Console.WriteLine("--------------------------------------------------");

                while (reader.Read())
                {
                    Console.WriteLine($"| {reader["pharmacy_name"],-18} | {reader["address"],-15} | {reader["phone_number"],-16} |");
                }

                Console.WriteLine("--------------------------------------------------");
            }

            Console.WriteLine("Pharmacy address updated successfully.");
        }
    }

}
