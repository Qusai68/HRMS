import { Component } from '@angular/core';
import { Employee } from '../../interfaces/employee';

@Component({
  selector: 'app-employees',
  imports: [],
  templateUrl: './employees.component.html',
  styleUrl: './employees.component.css'
})
export class EmployeesComponent {

  employees: Employee[] = [
    { id: 1, name: "emp1" }
  ];
  employeesTableColumns :string[]=[
     "#",
    "Name",
    "Position",
    "BirthDate",
    "Status",
    "Salary",
    "Email",
    "Department",
    "Manager"
    ]

}