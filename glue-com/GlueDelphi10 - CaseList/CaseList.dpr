program CaseList;

uses
  Vcl.Forms,
  Main in 'Main.pas' {Form1},
  GlueHelper in 'GlueHelper.pas',
  StringGridEx in 'StringGridEx.pas',
  GlueCOM_TLB in 'GlueCOM_TLB.pas',
  mscorlib_TLB in 'mscorlib_TLB.pas';

{$R *.res}

begin
  Application.Initialize;
  Application.MainFormOnTaskbar := True;
  Application.CreateForm(TForm1, Form1);
  Application.Run;
end.
