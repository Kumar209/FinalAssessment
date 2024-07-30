import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ActivatingAccountPageComponent } from './activating-account-page.component';

describe('ActivatingAccountPageComponent', () => {
  let component: ActivatingAccountPageComponent;
  let fixture: ComponentFixture<ActivatingAccountPageComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ActivatingAccountPageComponent]
    });
    fixture = TestBed.createComponent(ActivatingAccountPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
