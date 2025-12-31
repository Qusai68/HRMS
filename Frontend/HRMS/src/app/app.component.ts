import { Component } from '@angular/core';
 import { RouterOutlet } from '@angular/router';
import { NgClass ,NgIf ,NgFor } from '@angular/common';
import { EmployeesComponent } from './compoents/employees/employees.component';
@Component({
  selector: 'app-root',
  imports: [RouterOutlet, EmployeesComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  
}