import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmailSendPageComponent } from './email-send-page.component';

describe('EmailSendPageComponent', () => {
  let component: EmailSendPageComponent;
  let fixture: ComponentFixture<EmailSendPageComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [EmailSendPageComponent]
    });
    fixture = TestBed.createComponent(EmailSendPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
