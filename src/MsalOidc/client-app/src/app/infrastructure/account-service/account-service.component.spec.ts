import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountServiceComponent } from './account-service.component';

describe('AccountServiceComponent', () => {
  let component: AccountServiceComponent;
  let fixture: ComponentFixture<AccountServiceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccountServiceComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AccountServiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
